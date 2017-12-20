using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Models;
using Microsoft.AspNet.Identity;
using LonghornBank.StockUtilities;

namespace LonghornBank.Controllers
{
    public class StockPortfoliosController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: StockPortfolios
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            decimal? GetStockBalance1 = GetStockBalance();
            string GetStatus1 = GetStatus();
            db.SaveChanges();
            return View(u);

        }
        //HTTP.Get: StockPortfolios/Buy
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public ActionResult Buy(string Ticker, int? NumShares, string TickerName, string SelectedAccount, string Date)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            var queryz = from s in db.StockPortfolios
                         where s.AppUser.Id == user
                         select s.CanBuy;
            List<bool> BuyStatus = queryz.ToList();
            if (BuyStatus[0] == false)
            {
                return RedirectToAction("StocksOwned", "StockPortfolios");
            }
            ViewBag.Ticker = GetPossibleStocks();
            ViewBag.NumShares = NumShares;
            ViewBag.SelectedAccount = GetPossibleAccounts();
            return View();
        }
        //HttpPost: StockPortfolios/Buy
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(string SelectTicker, int? NumShares, string SelectAccount, string Date)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
                DateTime TransactionDate = Convert.ToDateTime(Date);
                //Finds the fee amount for the specified Transaction
                int final = SelectTicker.LastIndexOf(',');
                string subfee = SelectTicker.Substring(final + 2, 5);
                decimal fee = Convert.ToDecimal(subfee);
                //Sets the ending index of a comma so Substring knows where to parse
                int end = SelectTicker.IndexOf(',');
                string SelectedTicker = SelectTicker.Substring(0, end);
                string sub = SelectAccount.Substring(0, 10);
                //Converts the parsed SelectTicker String into the required Account Number
                Int32 SelectedAccount = Convert.ToInt32(sub);

                //Finds the stock portfolio associated with the logged in user and gets its ID
                var f = from s in db.StockPortfolios
                        where s.AppUser.Id == user
                        select s.StockPortfolioID;
                List<Int32> Temporary = f.ToList();
                Int32 ChosenStockID = Temporary[0];

                //Creates a list of stock quotes for information to be passed into
                List<StockQuote> Quotes = new List<StockQuote>();

                //Finds the stock information associated with the selected ticker and puts those tickers into list
                var query2 = from s in db.StockLists
                             where s.Ticker == SelectedTicker
                             select s.Ticker;

                List<string> StockTickers = query2.ToList();

                //Finds the stocklist ID associated with the given ticker
                var query3 = from s in db.StockLists
                             where s.Ticker == SelectedTicker
                             select s.StockListID;
                List<Int32> StockTickerID = query3.ToList();

                //Grabs the correct Stock List Item associated with the ticker
                StockList ChosenStockList = db.StockLists.Find(StockTickerID[0]);

                //Finds the updated information on the selected Stock 
                foreach (string Ticker in StockTickers)
                {
                    StockQuote sq = GetQuote.GetStock(Ticker, TransactionDate);
                    Quotes.Add(sq);
                }

                //Searches Stock Portfolios for the account number, does updating
                var query = from s in db.StockPortfolios
                            where SelectedAccount == s.StockPortfolioNumber
                            select s.StockPortfolioID;
                List<Int32> querya = query.ToList();
                if (querya.Count == 1)
                {
                    if (ModelState.IsValid)
                    {
                        StockPortfolio ToBuy = db.StockPortfolios.Find(querya[0]);
                        if (Convert.ToDecimal(Quotes[0].PreviousClose * NumShares) + fee >= ToBuy.CashBalance)
                        {
                            return RedirectToAction("Error", "StockPortfolios");
                        }
                        else
                        {
                            //Buys the stock from the cash balance
                            StockBridge NewStockBridge = new StockBridge();
                            ToBuy.StockBridges.Add(NewStockBridge);
                            ChosenStockList.StockBridges.Add(NewStockBridge);
                            //Need to find way to update all other fields, preferably by importing data from Stockquote
                            NewStockBridge.NumberShares = NumShares;
                            NewStockBridge.OriginalPrice = Convert.ToDecimal(Quotes[0].PreviousClose);
                            ToBuy.StockBalance += (GetStockBalance() + (Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            ToBuy.CashBalance -= (Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares);
                            ToBuy.CashBalance -= fee;
                            ToBuy.TotalFees += fee;
                            NewStockBridge.PurchaseDate = DateTime.Today;
                            db.SaveChanges();


                            //Creates a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AppUser = u;
                            transactionToChange.AccountSenderID = ToBuy.StockPortfolioID;
                            transactionToChange.TransactionAmount = Convert.ToDecimal((Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            transactionToChange.TransactionDescription = Convert.ToString("Stock Purchase " + Convert.ToString(SelectedAccount));
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            transactionToChange.AccountSender = ToBuy.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);
                            db.SaveChanges();

                            //Creates a fee Transaction
                            Transaction transactionToChange1 = new Transaction();
                            transactionToChange1.AppUser = u;
                            transactionToChange1.AccountSenderID = ToBuy.StockPortfolioID;
                            transactionToChange1.TransactionAmount = fee;
                            transactionToChange1.TransactionDescription = Convert.ToString("Fee: " + ChosenStockList.Name);
                            // TODO: Not today, should be user input
                            transactionToChange1.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange1.TypeOfTransaction = TransactionType.Fee;
                            transactionToChange1.AccountSender = ToBuy.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange1);
                            db.SaveChanges();
                            Success(string.Format("Stock successfully purchased."), true);
                            return RedirectToAction("StocksOwned", "StockPortfolios");
                        }
                    }
                }
                //Same thing but checks Checkings accounts rather than stock portfolios
                var query4 = from s in db.Checkings
                             where SelectedAccount == s.CheckingNumber
                             select s.CheckingID;
                List<Int32> queryb = query4.ToList();
                if (queryb.Count == 1)
                {
                    if (ModelState.IsValid)
                    {
                        Checking ToBuy = db.Checkings.Find(queryb[0]);
                        StockPortfolio StockPort = db.StockPortfolios.Find(ChosenStockID);
                        if (Convert.ToDecimal(Quotes[0].PreviousClose * NumShares) + fee >= ToBuy.CheckingBalance || fee > StockPort.CashBalance)
                        {
                            return RedirectToAction("Error", "StockPortfolios");
                        }
                        else
                        {
                            StockBridge NewStockBridge = new StockBridge();

                            StockPort.StockBridges.Add(NewStockBridge);
                            ChosenStockList.StockBridges.Add(NewStockBridge);
                            //Need to find way to update all other fields, preferably by importing data from Stockquote
                            NewStockBridge.NumberShares = NumShares;
                            NewStockBridge.OriginalPrice = Convert.ToDecimal(Quotes[0].PreviousClose);
                            StockPort.StockBalance += (GetStockBalance() + (Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            StockPort.CashBalance -= fee;
                            StockPort.TotalFees += fee;
                            ToBuy.CheckingBalance -= (Convert.ToDecimal(Quotes[0].PreviousClose) * Convert.ToDecimal(NumShares));
                            NewStockBridge.PurchaseDate = DateTime.Today;
                            db.SaveChanges();

                            //Creates a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AppUser = u;
                            // FIX THIS
                            transactionToChange.AccountSenderID = ToBuy.CheckingID;
                            transactionToChange.TransactionAmount = Convert.ToDecimal((Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            transactionToChange.TransactionDescription = Convert.ToString("Stock Purchase " + Convert.ToString(SelectedAccount));
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            // FIX THIS
                            transactionToChange.AccountSender = ToBuy.CheckingNumber;
                            db.Transactions.Add(transactionToChange);
                            db.SaveChanges();

                            //Creates a fee Transaction
                            Transaction transactionToChange1 = new Transaction();
                            transactionToChange1.AppUser = u;
                            transactionToChange1.TransactionAmount = fee;
                            transactionToChange1.AccountSenderID = StockPort.StockPortfolioID;
                            transactionToChange1.TransactionDescription = Convert.ToString("Fee: " + ChosenStockList.Name);
                            // TODO: Not today, should be user input
                            transactionToChange1.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange1.TypeOfTransaction = TransactionType.Fee;
                            transactionToChange1.AccountSender = StockPort.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange1);
                            db.SaveChanges();

                            Success(string.Format("Stock successfully purchased."), true);
                            return RedirectToAction("StocksOwned", "StockPortfolios");
                        }
                    }
                }
                //Checks to see if selected account is a savings account
                var query5 = from s in db.Savings
                             where SelectedAccount == s.SavingsNumber
                             select s.SavingsID;
                List<Int32> queryc = query5.ToList();
                if (queryc.Count == 1)
                {
                    if (ModelState.IsValid)
                    {
                        Savings ToBuy = db.Savings.Find(queryc[0]);
                        StockPortfolio StockPort = db.StockPortfolios.Find(ChosenStockID);
                        if (Convert.ToDecimal(Quotes[0].PreviousClose * NumShares) + fee >= ToBuy.SavingsBalance || fee > StockPort.CashBalance)
                        {
                            return RedirectToAction("Error", "StockPortfolios");
                        }
                        else
                        {
                            StockBridge NewStockBridge = new StockBridge();
                            StockPort.StockBridges.Add(NewStockBridge);
                            ChosenStockList.StockBridges.Add(NewStockBridge);
                            //Need to find way to update all other fields, preferably by importing data from Stockquote
                            NewStockBridge.NumberShares = NumShares;
                            NewStockBridge.OriginalPrice = Convert.ToDecimal(Quotes[0].PreviousClose);
                            StockPort.StockBalance += (GetStockBalance() + (Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            ToBuy.SavingsBalance -= (Convert.ToDecimal(Quotes[0].PreviousClose) * Convert.ToDecimal(NumShares));
                            StockPort.CashBalance -= fee;
                            StockPort.TotalFees += fee;
                            NewStockBridge.PurchaseDate = Convert.ToDateTime(Date);
                            db.SaveChanges();

                            //Creates a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AppUser = u;
                            // FIX THIS
                            transactionToChange.AccountSenderID = ToBuy.SavingsID;
                            transactionToChange.TransactionAmount = Convert.ToDecimal((Convert.ToDecimal(Quotes[0].PreviousClose) * NumShares));
                            transactionToChange.TransactionDescription = Convert.ToString("Stock Purchase " + Convert.ToString(SelectedAccount));
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            // FIX THIS
                            transactionToChange.AccountSender = ToBuy.SavingsNumber;
                            db.Transactions.Add(transactionToChange);
                            db.SaveChanges();

                            //Creates a fee Transaction
                            Transaction transactionToChange1 = new Transaction();
                            transactionToChange1.AppUser = u;
                            transactionToChange1.TransactionAmount = fee;
                            transactionToChange1.AccountSenderID = StockPort.StockPortfolioID;
                            transactionToChange1.TransactionDescription = Convert.ToString("Fee: " + ChosenStockList.Name);
                            // TODO: Not today, should be user input
                            transactionToChange1.TransactionDate = Convert.ToDateTime(Date);
                            transactionToChange1.TypeOfTransaction = TransactionType.Fee;
                            transactionToChange1.AccountSender = StockPort.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange1);
                            db.SaveChanges();

                            Success(string.Format("Stock successfully purchased."), true);
                            return RedirectToAction("StocksOwned", "StockPortfolios");
                        }
                    }
                }
            var queryab = from s in db.Users
                        where user == s.Id
                        select s.StockPortfolios.ToList();
            List<List<StockPortfolio>> Temp = queryab.ToList();
            List<StockPortfolio> Temp1 = Temp[0];
            foreach (var item in Temp1)
            {
                item.StockBalance = GetStockBalance();
                item.BalanceStatus = GetStatus();
            }
            db.SaveChanges();
            return View();
            
        }
        //HttpGet: StockPortfolios/Sell
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public ActionResult Sell(string StockBridgeID, string Name, int? NumShares, decimal Gain, string Ticker)
        {
            string user = User.Identity.GetUserId();
            var query = from s in db.StockLists
                        where s.Ticker == Ticker
                        select s.TransactionFee;
            List<Decimal> Fees = query.ToList();
            ViewBag.Fee = Fees[0];
            ViewBag.Name = Name;
            ViewBag.NumShares = NumShares;
            ViewBag.Gain = Gain;
            ViewBag.StockInfo = StockBridgeID;
            ViewBag.Ticker = Ticker;
            return View();
        }

        //HttpPost: StockPortfolios/Sell
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public ActionResult Sell( int? NumShares, string SBIDA)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            Int32 SBID = Convert.ToInt32(SBIDA);
            var querya = from s in db.StockPortfolios
                         where user == s.AppUser.Id
                         select s.StockPortfolioID;
            List<Int32> Tempo = querya.ToList();
            StockPortfolio Used = db.StockPortfolios.Find(Tempo[0]);
            List < StockBridge > SBridges = Used.StockBridges;
            List<StockBridge> Chosen = new List<StockBridge>();
            foreach (var item in SBridges)
            {
                if (item.StockBridgeID == (SBID))
                {
                    Chosen.Add(item);
                }
            }
            string ChosenTicker = Chosen[0].StockList.Ticker;
            StockQuote sq = GetQuote.GetStock(ChosenTicker);
            if (Chosen[0].NumberShares >= NumShares)
            {
                Chosen[0].NumberShares -= NumShares;
                Used.CashBalance += Convert.ToDecimal(NumShares * (Convert.ToDecimal(sq.PreviousClose)));
                Used.CashBalance -= Chosen[0].StockList.TransactionFee;
                Used.TotalFees += Chosen[0].StockList.TransactionFee;
                db.SaveChanges();

                //Creates a transaction
                Transaction transactionToChange = new Transaction();
                transactionToChange.AppUser = u;
                transactionToChange.AccountSenderID = Used.StockPortfolioID;
                transactionToChange.TransactionAmount = Convert.ToDecimal((Convert.ToDecimal(sq.PreviousClose) * NumShares));
                transactionToChange.TransactionDescription = Convert.ToString("Stock Name: "+Chosen[0].StockList.Name+"  Shares sold: "+NumShares+"  Initial Price: "+Chosen[0].OriginalPrice+"  Current Price: "+sq.PreviousClose+"  Total Gains/Losses: "+NumShares*(Convert.ToDecimal(sq.PreviousClose)-Chosen[0].OriginalPrice));
                // TODO: Not today, should be user input
                transactionToChange.TransactionDate = (DateTime.Today);
                transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                transactionToChange.AccountSender = Convert.ToInt32(Chosen[0].StockPortfolio.StockPortfolioNumber);
                db.Transactions.Add(transactionToChange);
                db.SaveChanges();

                //Creates a fee Transaction
                Transaction transactionToChange1 = new Transaction();
                transactionToChange1.AppUser = u;
                transactionToChange1.AccountSenderID = Used.StockPortfolioID;
                transactionToChange1.TransactionAmount = Chosen[0].StockList.TransactionFee;
                transactionToChange1.TransactionDescription = Convert.ToString("Fee for sale of "+Chosen[0].StockList.Name);
                // TODO: Not today, should be user input
                transactionToChange1.TransactionDate = (DateTime.Today);
                transactionToChange1.TypeOfTransaction = TransactionType.Fee;
                transactionToChange1.AccountSender = Convert.ToInt32(Chosen[0].StockPortfolio.StockPortfolioNumber);
                db.Transactions.Add(transactionToChange1);
                db.SaveChanges();

            }
            else
            {
                return RedirectToAction("Error", "StockPortfolios");
            }
            string Tempo1 = GetStatus();
            var queryab = from s in db.Users
                          where user == s.Id
                          select s.StockPortfolios.ToList();
            List<List<StockPortfolio>> Temp = queryab.ToList();
            List<StockPortfolio> Temp1 = Temp[0];
            foreach (var item in Temp1)
            {
                item.StockBalance = GetStockBalance();
                item.BalanceStatus = GetStatus();
            }
            db.SaveChanges();

            db.SaveChanges();
            Success(string.Format("Stock successfully sold."), true);
            return RedirectToAction("StocksOwned", "StockPortfolios");

        }
        public ActionResult Details(string StockBridgeID)
        {
            string user = User.Identity.GetUserId();
            Int32 ChosenBridgeID = Convert.ToInt32(StockBridgeID);
            var query = from s in db.StockPortfolios
                        where s.AppUser.Id == user
                        select s.StockBridges.ToList();
            List<List<StockBridge>> Temporary = query.ToList();
            List<StockBridge> UsedBridges = Temporary[0];
            foreach (var item in UsedBridges)
            {
                if (item.StockBridgeID == ChosenBridgeID)
                {
                    StockQuote sq = GetQuote.GetStock(item.StockList.Ticker);
                    ViewBag.Name = item.StockList.Name;
                    ViewBag.PurchasePrice = item.OriginalPrice;
                    ViewBag.Change = Convert.ToDecimal(sq.PreviousClose) - item.OriginalPrice;
                    ViewBag.Symbol = item.StockList.Ticker;
                }
            }
            return View();
        }

        //HttpGet: View Stocks Owned
        [Authorize(Roles = "Customer")]
        public ActionResult StocksOwned()
        {
            //Gets user ID based on signed in user, and pulls their stock portfolio
            string user = User.Identity.GetUserId();
            var query = from s in db.StockPortfolios
                        where s.AppUser.Id == user
                        select s.StockPortfolioID;
            List<Int32> Newlist = query.ToList();
            //Grabs the StockPortfolioID from that user's stock portfolio
            Int32 StockPortID = Newlist[0];

            //Get the associated Stock bridges
            var query2 = from s in db.StockPortfolios
                         where s.StockPortfolioID == StockPortID
                         select s.StockBridges;
            List<List<StockBridge>> Bridge1 = query2.ToList();
            List<StockBridge> Holder = Bridge1[0];
            List<StockBridge> Bridge = new List<StockBridge>();
            foreach (var item in Holder)
            {
                Bridge.Add(item);
            }
            List<StockViewModel> Views = new List<StockViewModel>();
            foreach (var item in Bridge)
            {
                if (item.NumberShares != 0)
                {
                    //Create an instance of the StockViewModel Model
                    StockViewModel entry = new StockViewModel();
                    //Selects the name of the stock associated with the current instance of Bridge
                    var query3 = from s in db.StockLists
                                 where item.StockList.StockListID == s.StockListID
                                 select s.Name;
                    //Selects the Ticker symbol of the stock associated with the current instance of Bridge
                    var query4 = from s in db.StockLists
                                 where item.StockList.StockListID == s.StockListID
                                 select s.Ticker;
                    List<string> Temp = query4.ToList();
                    StockQuote sq = GetQuote.GetStock(Temp[0]);
                    List<string> Temporary = query3.ToList();
                    entry.StockName = Temporary[0];
                    entry.Symbol = Temp[0];
                    entry.PurchasePrice = item.OriginalPrice;
                    entry.NumShares = Convert.ToInt32(item.NumberShares);
                    entry.CurrentPrice = Convert.ToDecimal(sq.PreviousClose);
                    entry.Difference = entry.PurchasePrice - entry.CurrentPrice;
                    entry.TotalChange = (entry.PurchasePrice - entry.CurrentPrice) * entry.NumShares;
                    entry.StockBridgeID = Convert.ToString(item.StockBridgeID);
                    Views.Add(entry);
                } 
            }
            ViewBag.Views = Views;
            return View(Views);
        }
        [ValidateAntiForgeryToken]
        public SelectList GetPossibleAccounts()
        {

            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> Selected = new List<string>();
            var query = (from s in db.Checkings
                         where s.AppUser.Id == user
                         select new { value1 = s.CheckingNumber, value2 = s.CheckingBalance, value3 = s.CheckingName }).Union(from sp in db.Checkings
                                                                                                                              where sp.AppUser.Id ==user
                                                                                                     select new { value1 = sp.CheckingNumber, value2 = sp.CheckingBalance, value3 = sp.CheckingName }).Union(from spq in db.Checkings
                                                                                                                                                                                                             where spq.AppUser.Id == user
                                                                                                                                                                                   select new { value1 = spq.CheckingNumber, value2 = spq.CheckingBalance, value3 = spq.CheckingName });
            var query2 = (from s in db.Savings
                          where s.AppUser.Id == user
                         select new { value1 = s.SavingsNumber, value2 = s.SavingsBalance, value3 = s.SavingsName }).Union(from sp in db.Savings
                                                                                                                           where sp.AppUser.Id == user
                                                                                                   select new { value1 = sp.SavingsNumber, value2 = sp.SavingsBalance, value3 = sp.SavingsName }).Union(from spq in db.Savings
                                                                                                                                                                                                        where spq.AppUser.Id == user
                                                                                                                                                                                                       select new { value1 = spq.SavingsNumber, value2 = spq.SavingsBalance, value3 = spq.SavingsName });
            var query3 = (from s in db.StockPortfolios
                          where s.AppUser.Id == user
                         select new { value1 = s.StockPortfolioNumber, value2 = s.CashBalance, value3 = s.StockPortfolioName }).Union(from sp in db.StockPortfolios
                                                                                                                                      where sp.AppUser.Id == user
                                                              select new { value1 = sp.StockPortfolioNumber, value2 = sp.CashBalance, value3 = sp.StockPortfolioName }).Union(from spq in db.StockPortfolios
                                                                                                                                                                              where spq.AppUser.Id == user
                                                                                                                                                                             select new { value1 = spq.StockPortfolioNumber, value2 = spq.CashBalance, value3 = spq.StockPortfolioName });
            foreach (var item in query)
            {
                Selected.Add(Convert.ToString(item.value1)+", "+Convert.ToString(item.value2)+", "+item.value3);
            }
            foreach (var item in query2)
            {
                Selected.Add(Convert.ToString(item.value1)+", "+Convert.ToString(item.value2)+", "+item.value3);
            }
            foreach (var item in query3)
            {
                Selected.Add(Convert.ToString(item.value1)+", "+ Convert.ToDecimal(item.value2)+", "+item.value3);
            }


            //convert list to select list format needed for HTML
            SelectList selectedBankingType = new SelectList(Selected);
            return selectedBankingType;
        }
        public SelectList GetPossibleStocks()
        {
            List<StockQuote> Quotes = new List<StockQuote>();
            List<string> TempList = new List<string>();
            List<string> Newlist = new List<string>();
            var query = (from s in db.StockLists
                        select new { value1 = s.Ticker, value2 = s.StockType, value3 = s.TransactionFee }).Union(from sp in db.StockLists
                                                                                                                 select new { value1 = sp.Ticker, value2 = sp.StockType, value3 = sp.TransactionFee }).Union(from spz in db.StockLists
                                                                                                                                                                                                             select new { value1 = spz.Ticker, value2 = spz.StockType, value3 = spz.TransactionFee });
            foreach (var item in query)
            {
                TempList.Add(item.value1+"," + Convert.ToString(item.value2)+","+Convert.ToString(item.value3));
            }
            foreach (string Ticker in TempList)
            {
                int final = Ticker.Length;
                int begin = Ticker.IndexOf(',');
                int end = Ticker.LastIndexOf(',');
                string Tick = Ticker.Substring(0, begin);
                StockQuote sq = GetQuote.GetStock(Tick);
                sq.Type = Ticker.Substring(begin + 1, end-begin-1);
                string finalfee1 = Ticker.Substring(end + 1, final-end-1);
                decimal finalfee = Convert.ToDecimal(finalfee1);
                sq.Fee = finalfee;
                Quotes.Add(sq);
            }
            foreach (StockQuote quote in Quotes)
            {
                Newlist.Add(quote.Symbol + ", " + Convert.ToString(quote.PreviousClose)+" per share, " +quote.Name + ", " + quote.Type + ", " + Convert.ToString(quote.Fee)+" per purchase");
            }
            SelectList selectedStock = new SelectList(Newlist);
            return selectedStock;
        }
        public ActionResult Error()
        {
            return View();
        }
        public SelectList GetPurchasedStocks()
        {
            string user = User.Identity.GetUserId();
            var query2 = from s in db.StockPortfolios
                         where s.AppUser.Id == user
                         select s.StockBridges;
            List<List<StockBridge>> Bridge = query2.ToList();
            List<StockBridge> BridgeItem = Bridge[0];
            List<string> Names = new List<string>();
            foreach (var item in BridgeItem)
            {
                if(Names.Contains(item.StockList.Name))
                {
                    Console.WriteLine("HAHAHA");
                }
                else
                {
                    Names.Add(item.StockList.Name);
                }
            }

            SelectList selectedBankingType = new SelectList(Names);
            return (selectedBankingType);
        }
        public decimal? GetStockBalance()
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            var query2 = from s in db.StockPortfolios
                         where s.AppUser.Id == user
                         select s.StockBridges;
            List < List < StockBridge >> Bridge = query2.ToList();
            List<StockBridge> BridgeItem = Bridge[0];
            decimal? Total = 0m;
            foreach (var item in BridgeItem)
            {
                StockQuote sq = GetQuote.GetStock(item.StockList.Ticker);
                Decimal CurrentPrice = Convert.ToDecimal(sq.PreviousClose);
                Total += Convert.ToDecimal((item.NumberShares * CurrentPrice));
            }            

            return (Total);
        }
        public String GetStatus()
        {
            //Sets status of balance/unbalance
            string status = "";

            //Grabs logged in user Id
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            //Queries for Associated Stock Portfolio Bridges (stock transactions)
            var query2 = from s in db.StockPortfolios
                         where s.AppUser.Id == user
                         select s.StockBridges;

            //Queries for associated Stock portfolio with the user
            var query3 = from s in db.Users
                         where s.Id == user
                         select s.StockPortfolios.ToList() ;

            //Got the associated portfolio with the user and calls it "used"
            List <List<StockPortfolio>> temp = query3.ToList();
            List<StockPortfolio> temp2 = temp[0];
            StockPortfolio used = temp2[0];

            //Gets the associated stock bridges and puts them in a list called "bridgeitem"
            List < List < StockBridge >> Bridge = query2.ToList();
            List<StockBridge> BridgeItem = Bridge[0];
            Int32 OrdinaryCount = 0;
            Int32 IndexCount = 0;
            Int32 ETFCount = 0;
            Int32 MutualCount = 0;
            Int32 FuturesCount = 0;
            foreach (var item in BridgeItem)
            {
                if (Convert.ToString(item.StockList.StockType) == "Ordinary" && item.NumberShares!= 0)
                {
                    OrdinaryCount += 1;
                }
                else if (Convert.ToString(item.StockList.StockType) == "Index" && item.NumberShares != 0)
                {
                    IndexCount += 1;
                }
                else if (Convert.ToString(item.StockList.StockType) == "ETF" && item.NumberShares != 0)
                {
                    ETFCount += 1;
                }
                else if (Convert.ToString(item.StockList.StockType) == "Mutual" && item.NumberShares != 0)
                {
                    MutualCount += 1;
                }
                else
                {
                    FuturesCount += 1;
                }
            }
            if (OrdinaryCount >=2 && IndexCount >=1 && MutualCount >= 1)
            {
                status = "Balanced";
                used.BalanceStatus = "Balanced";
                db.Entry(used).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                status = "UnBalanced";
                used.BalanceStatus = "UnBalanced";
                db.Entry(used).State = EntityState.Modified;
                db.SaveChanges();
            }
            return (status);
        }
    }
}

