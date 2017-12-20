using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Controllers;
using LonghornBank.Models;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.Data.Entity;
using System.Data;
using LonghornBank.StockUtilities;
using System.Text.RegularExpressions;


namespace LonghornBank.Controllers
{
    public class ManageAccountsController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: ManageAccounts/Index
        [Authorize(Roles = "Manager, Customer, Employee")]
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            if (u.StockPortfolios != null)
            {
                var query = from s in db.Users
                            where user == s.Id
                            select s.StockPortfolios.ToList();
                List<List<StockPortfolio>> Temp = query.ToList();
                List<StockPortfolio> Temp1 = Temp[0];
                foreach (var item in Temp1)
                {
                    item.StockBalance = GetStockBalance();
                }
            }

           



            return View(u);
        }
        //Gets the current balance of stock portfolio
        public Decimal GetStockBalance()
        {
            string user = User.Identity.GetUserId();
            var query2 = from s in db.StockPortfolios
                         where s.AppUser.Id == user
                         select s.StockBridges;
            List<List<StockBridge>> Bridge = query2.ToList();
            List<StockBridge> BridgeItem = Bridge[0];
            Decimal Total = 0m;
            foreach (var item in BridgeItem)
            {
                StockQuote sq = GetQuote.GetStock(item.StockList.Ticker);
                Decimal CurrentPrice = Convert.ToDecimal(sq.PreviousClose);
                Total += Convert.ToDecimal((item.NumberShares * CurrentPrice));
            }

            return (Total);
        }


        // GET: ManageAccounts/Deposit
        [Authorize(Roles = "Customer")]
        public ActionResult Deposit(string SelectedType, int id, int AccountNumber)
        {
            ViewBag.Id = id;
            ViewBag.AccountType = SelectedType;
            ViewBag.AccountNumber = AccountNumber;
            return View();
        }

        // POST: ManageAccounts/Deposit
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit(string TransactionDate, string SelectedAccountType, int Id, string SelectedAmount, int AccountNumber, TransactionStatus? ApprovalStatus)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            if (Convert.ToDecimal(SelectedAmount) > 0)
            {

                if (SelectedAccountType == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //if the amount is over pass it onto the pending transaction page
                        //do not add any amount to their account
                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            Pending pendingToChange = new Pending();
                            pendingToChange.AccountNumber = AccountNumber;
                            pendingToChange.AccountType = SelectedAccountType;
                            pendingToChange.AppUser = u;
                            pendingToChange.PendingStatus = PendingStatus.Submitted;
                            pendingToChange.Amount = Convert.ToDecimal(SelectedAmount);
                            db.Pendings.Add(pendingToChange);
                        }
                        else
                        {
                            //Find associated account
                            Checking accountToChange = db.Checkings.Find(Id);
                            accountToChange.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;
                        }

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = u;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "this is a deposit what more do you want";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                        transactionToChange.AccountSender = AccountNumber;

                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                        }
                        else
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                        }

                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //if the amount is over pass it onto the pending transaction page
                        //do not add any amount to their account
                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            Pending pendingToChange = new Pending();
                            pendingToChange.AccountNumber = AccountNumber;
                            pendingToChange.AccountType = SelectedAccountType;
                            pendingToChange.AppUser = u;
                            pendingToChange.PendingStatus = PendingStatus.Submitted;
                            pendingToChange.Amount = Convert.ToDecimal(SelectedAmount);
                            db.Pendings.Add(pendingToChange);
                        }
                        else
                        {
                            //Find associated account
                            Savings accountToChange = db.Savings.Find(Id);
                            accountToChange.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;
                        }

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = u;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "this is a deposit what more do you want";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                        transactionToChange.AccountSender = AccountNumber;

                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                        }
                        else
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                        }

                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "IRA")
                {
                    if (ModelState.IsValid)
                    {

                        //Find associated account
                        IRA accountToChange = db.IRAs.Find(Id);
                        if ((accountToChange.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange.Contribution), id = Id, AccountNumber = AccountNumber });
                        }
                        else
                        {
                            accountToChange.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange).State = EntityState.Modified;


                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = u;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "this is a deposit what more do you want";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                        transactionToChange.AccountSender = AccountNumber;

                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                        }
                        else
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                        }

                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //if the amount is over pass it onto the pending transaction page
                        //do not add any amount to their account
                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            Pending pendingToChange = new Pending();
                            pendingToChange.AccountNumber = AccountNumber;
                            pendingToChange.AccountType = SelectedAccountType;
                            pendingToChange.AppUser = u;
                            pendingToChange.PendingStatus = PendingStatus.Submitted;
                            pendingToChange.Amount = Convert.ToDecimal(SelectedAmount);
                            db.Pendings.Add(pendingToChange);
                        }
                        else
                        {
                            //Find associated account
                            StockPortfolio accountToChange = db.StockPortfolios.Find(Id);
                            accountToChange.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;
                        }


                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = u;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "this is a deposit what more do you want";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                        transactionToChange.AccountSender = AccountNumber;

                        if (Convert.ToDecimal(SelectedAmount) > 5000.0m)
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                        }
                        else
                        {
                            transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                        }

                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
              Danger(string.Format("Warning: The Deposit must be greater than 0."), true);
              return RedirectToAction("Index");
            }
        }

        // GET: ManageAccounts/DepositLimit
        [Authorize(Roles = "Customer")]
        public ActionResult DepositLimit(string TransactionDate, int id, string SelectedAmount, int AccountNumber)
        {
            ViewBag.AccountType = "IRA";
            ViewBag.AccountNumber = AccountNumber;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/DepositLimit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepositLimit(string TransactionDate, string SelectedAccountType, int Id, string SelectedAmount, int AccountNumber)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            if (ModelState.IsValid)
            {

                //Find associated account
                IRA accountToChange = db.IRAs.Find(Id);
                if ((accountToChange.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                {
                    return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange.Contribution), id = Id, AccountNumber = AccountNumber });
                }
                else
                {
                    accountToChange.IRABalance += Convert.ToDecimal(SelectedAmount);
                    accountToChange.Contribution += Convert.ToDecimal(SelectedAmount);
                }
                db.Entry(accountToChange).State = EntityState.Modified;

                //Create a transaction
                Transaction transactionToChange = new Transaction();
                transactionToChange.AccountSenderID = Id;
                transactionToChange.AppUser = u;
                transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                transactionToChange.TransactionDescription = "this is a deposit what more do you want";
                // TODO: Not today, should be user input
                transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                transactionToChange.AccountSender = AccountNumber;
                db.Transactions.Add(transactionToChange);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: ManageAccounts/Withdraw
        [Authorize(Roles = "Customer")]
        public ActionResult Withdraw([Bind(Include = "TransactionDate")] Transaction @transaction, string SelectedType, int id, int AccountNumber)

        {
            ViewBag.Id = id;
            ViewBag.AccountType = SelectedType;
            ViewBag.AccountNumber = AccountNumber;
            return View();
        }

        // POST: ManageAccounts/Withdraw
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Withdraw(string TransactionDate, string SelectedAccountType, int Id, string SelectedAmount, int AccountNumber)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            if (Convert.ToDecimal(SelectedAmount) > 0)
            {
                if (SelectedAccountType == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account
                        Checking accountToChange = db.Checkings.Find(Id);
                        if (Convert.ToDecimal(SelectedAmount) <= accountToChange.CheckingBalance)
                        {
                            accountToChange.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id;
                            transactionToChange.AppUser = accountToChange.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "this is a withdrawal what more do you want";
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            transactionToChange.AccountSender = AccountNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Danger(string.Format("You may not withdraw more than your account balance."), true);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account
                        Savings accountToChange = db.Savings.Find(Id);
                        if (Convert.ToDecimal(SelectedAmount) <= accountToChange.SavingsBalance)
                        {
                            accountToChange.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id;
                            transactionToChange.AppUser = accountToChange.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "this is a withdrawal what more do you want";
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            transactionToChange.AccountSender = AccountNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Danger(string.Format("You may not withdraw more than your account balance."), true);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "IRA")
                {
                    if (ModelState.IsValid)
                    {
                        if ((Convert.ToDateTime(TransactionDate) - u.DOB).Days <= 23741.25m)
                        {
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", id = Id, AccountNumber = AccountNumber });
                            }
                            else
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = SelectedAmount, id = Id, AccountNumber = AccountNumber });
                            }
                        }
                        else
                        {
                            //Find associated account
                            IRA accountToChange = db.IRAs.Find(Id);
                            if (Convert.ToDecimal(SelectedAmount) <= accountToChange.IRABalance)
                            {
                                accountToChange.IRABalance -= Convert.ToDecimal(SelectedAmount);
                                db.Entry(accountToChange).State = EntityState.Modified;

                                //Create a transaction
                                Transaction transactionToChange = new Transaction();
                                transactionToChange.AccountSenderID = Id;
                                transactionToChange.AppUser = accountToChange.AppUser;
                                transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                                transactionToChange.TransactionDescription = "this is a withdrawal what more do you want";
                                // TODO: Not today, should be user input
                                transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                                transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                                transactionToChange.AccountSender = AccountNumber;
                                db.Transactions.Add(transactionToChange);
                                db.SaveChanges();
                            }
                            else
                            {
                                Danger(string.Format("You may not withdraw more than your account balance."), true);
                                return RedirectToAction("Index");
                            }
                        }

                        
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account
                        StockPortfolio accountToChange = db.StockPortfolios.Find(Id);
                        if (Convert.ToDecimal(SelectedAmount) <= accountToChange.CashBalance)
                        {
                            accountToChange.CashBalance -= Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id;
                            transactionToChange.AppUser = accountToChange.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "this is a withdrawal what more do you want";
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                            transactionToChange.AccountSender = AccountNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Danger(string.Format("You may not withdraw more than your account balance."), true);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Danger(string.Format("Warning: The Withdraw must be greater than 0."), true);
                return RedirectToAction("Index");
            }
        }




        // GET: ManageAccounts/WithdrawLimit
        [Authorize(Roles = "Customer")]
        public ActionResult WithdrawLimit(int id, string SelectedAmount, int AccountNumber)
        {
            ViewBag.AccountType = "IRA";
            ViewBag.AccountNumber = AccountNumber;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/WithdrawLimit
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult WithdrawLimit(string TransactionDate, string SelectedAccountType, int Id, string SelectedAmount, int AccountNumber, string SelectedInclude)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            if (ModelState.IsValid)
            {

                if (Convert.ToDecimal(SelectedAmount) > 3000)
                {
                    return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", id = Id, AccountNumber = AccountNumber });
                }
                else
                {
                    if(SelectedInclude == "Yes")
                    {
                        //Find associated account
                        IRA accountToChange = db.IRAs.Find(Id);
                        accountToChange.IRABalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount) - 30.0m;
                        transactionToChange.TransactionDescription = "unqualified distribution";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Withdrawal;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        //record fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id;
                        feeToChange.AppUser = accountToChange.AppUser;
                        feeToChange.TransactionAmount = 30.0m;
                        feeToChange.TransactionDescription = "unqualified distribution fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(feeToChange);
                    }
                    else
                    {
                        //Find associated account
                        IRA accountToChange = db.IRAs.Find(Id);
                        accountToChange.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30.0m);
                        db.Entry(accountToChange).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "unqualified distribution";
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Fee;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        //record fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id;
                        feeToChange.AppUser = accountToChange.AppUser;
                        feeToChange.TransactionAmount = 30.0m;
                        feeToChange.TransactionDescription = "unqualified distribution fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Withdrawal;
                        feeToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(feeToChange);
                    }

                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        // GET: ManageAccounts/Transfer
        [Authorize(Roles = "Customer")]
        public ActionResult Transfer(string SelectedType, int id, int AccountNumber)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);


            ViewBag.Id = id;
            ViewBag.AccountType = SelectedType;
            ViewBag.AccountNumber = AccountNumber;
            return View(u);
        }

        // GET: ManageAccounts/TransferConfirmation
        [Authorize(Roles = "Customer")]
        public ActionResult TransferConfirmation(string SelectedType, int id, int AccountNumber, string ToSelectedType, int Toid, int ToAccountNumber)
        {
            ViewBag.id = id;
            ViewBag.ToId = Toid;

            ViewBag.AccountType = SelectedType;
            string Actual = Convert.ToString(AccountNumber);
            string sub = Actual.Substring(6, 4);
            string used = "XXXXXX" + sub;
            ViewBag.AccountNumber = used;

            ViewBag.ToAccountType = ToSelectedType;
            string stringToAccountNumber = Convert.ToString(ToAccountNumber);
            string subToAccount = Actual.Substring(6, 4);
            string stringUsedToAccount = "XXXXXX" + sub;
            ViewBag.ToAccountNumber = stringUsedToAccount;
            return View();
        }

        // POST: ManageAccounts/TransferConfirmation
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult TransferConfirmation(string TransactionDate, string SelectedAccountType1, int id, string SelectedAccountType2, int ToId, string SelectedAmount, int AccountNumber, int ToAccountNumber)
        {
            if (SelectedAccountType1 == "Checking")
            {
                if (SelectedAccountType2 == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Checking accountToChange1 = db.Checkings.Find(id);
                        accountToChange1.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Checking accountToChange2 = db.Checkings.Find(ToId);
                        accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.CheckingNumber + " to " + accountToChange2.CheckingNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        Success(string.Format("Transfer completed successfully."), true);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Checking accountToChange1 = db.Checkings.Find(id);
                        accountToChange1.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Savings accountToChange2 = db.Savings.Find(ToId);
                        accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.CheckingNumber + " to " + accountToChange2.SavingsNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        Success(string.Format("Transfer completed successfully."), true);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "IRA")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Checking accountToChange1 = db.Checkings.Find(id);
                        accountToChange1.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        IRA accountToChange2 = db.IRAs.Find(ToId);
                        accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.CheckingNumber + " to " + accountToChange2.IRANumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        Success(string.Format("Transfer completed successfully."), true);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Checking accountToChange1 = db.Checkings.Find(id);
                        accountToChange1.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(ToId);
                        accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.CheckingNumber + " to " + accountToChange2.StockPortfolioNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        Success(string.Format("Transfer completed successfully."), true);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else if (SelectedAccountType1 == "Savings")
            {
                if (SelectedAccountType2 == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Savings accountToChange1 = db.Savings.Find(id);
                        accountToChange1.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Checking accountToChange2 = db.Checkings.Find(ToId);
                        accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.SavingsNumber + " to " + accountToChange2.CheckingNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        Success(string.Format("Transfer completed successfully."), true);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Savings accountToChange1 = db.Savings.Find(id);
                        accountToChange1.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Savings accountToChange2 = db.Savings.Find(ToId);
                        accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.SavingsNumber + " to " + accountToChange2.SavingsNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "IRA")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Savings accountToChange1 = db.Savings.Find(id);
                        accountToChange1.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        IRA accountToChange2 = db.IRAs.Find(ToId);
                        accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.SavingsNumber + " to " + accountToChange2.IRANumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        Savings accountToChange1 = db.Savings.Find(id);
                        accountToChange1.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(ToId);
                        accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.SavingsNumber + " to " + accountToChange2.StockPortfolioNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            else if (SelectedAccountType1 == "IRA")
            {
                if (SelectedAccountType2 == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        IRA accountToChange1 = db.IRAs.Find(id);
                        accountToChange1.IRABalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Checking accountToChange2 = db.Checkings.Find(ToId);
                        accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.IRANumber + " to " + accountToChange2.CheckingNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        IRA accountToChange1 = db.IRAs.Find(id);
                        accountToChange1.IRABalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Savings accountToChange2 = db.Savings.Find(ToId);
                        accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.IRANumber + " to " + accountToChange2.SavingsNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "IRA")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        IRA accountToChange1 = db.IRAs.Find(id);
                        accountToChange1.IRABalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        IRA accountToChange2 = db.IRAs.Find(ToId);
                        accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.IRANumber + " to " + accountToChange2.IRANumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        IRA accountToChange1 = db.IRAs.Find(id);
                        accountToChange1.IRABalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(ToId);
                        accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.IRANumber + " to " + accountToChange2.StockPortfolioNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            else if (SelectedAccountType1 == "Stock")
            {
                if (SelectedAccountType2 == "Checking")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        StockPortfolio accountToChange1 = db.StockPortfolios.Find(id);
                        accountToChange1.CashBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Checking accountToChange2 = db.Checkings.Find(ToId);
                        accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.StockPortfolioNumber + " to " + accountToChange2.CheckingNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Savings")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        StockPortfolio accountToChange1 = db.StockPortfolios.Find(id);
                        accountToChange1.CashBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        Savings accountToChange2 = db.Savings.Find(ToId);
                        accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.StockPortfolioNumber + " to " + accountToChange2.SavingsNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "IRA")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        StockPortfolio accountToChange1 = db.StockPortfolios.Find(id);
                        accountToChange1.CashBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        IRA accountToChange2 = db.IRAs.Find(ToId);
                        accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.StockPortfolioNumber + " to " + accountToChange2.IRANumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (SelectedAccountType2 == "Stock")
                {
                    if (ModelState.IsValid)
                    {
                        //Find associated account1
                        StockPortfolio accountToChange1 = db.StockPortfolios.Find(id);
                        accountToChange1.CashBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Find associated account2
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(ToId);
                        accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from account " + accountToChange1.StockPortfolioNumber + " to " + accountToChange2.StockPortfolioNumber;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = AccountNumber;
                        transactionToChange.AccountReceiver = ToAccountNumber;
                        db.Transactions.Add(transactionToChange);
                        Success(string.Format("Transfer completed successfully."), true);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        public SelectList GetAccounts(string AccountType)
        {

            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);


            if(AccountType == "Checking")
            {
                //create query to find all committees
                var query = from c in db.Checkings
                            where c.AppUser.Id == user
                            orderby c.CheckingNumber
                            select c;

                //execute query to store in list
                List<Checking> all = query.ToList();

                //convert list to select list format needed for HTML
                SelectList alllist = new SelectList(all, "CheckingID", "CheckingNumber");
                return alllist;

            }
            else if (AccountType == "Savings")
            {
                //create query to find all committees
                var query = from c in db.Savings
                            where c.AppUser.Id == user
                            orderby c.SavingsNumber
                            select c;

                //execute query to store in list
                List<Savings> all = query.ToList();

                //convert list to select list format needed for HTML
                SelectList alllist = new SelectList(all, "SavingsID", "SavingsNumber");
                return alllist;

            }
            else if (AccountType == "IRA")
            {
                //create query to find all committees
                var query = from c in db.IRAs
                            where c.AppUser.Id == user
                            orderby c.IRANumber
                            select c;

                //execute query to store in list
                List<IRA> all = query.ToList();

                //convert list to select list format needed for HTML
                SelectList alllist = new SelectList(all, "IRAID", "IRANumber");
                return alllist;

            }
            else
            {
                //create query to find all committees
                var query = from c in db.StockPortfolios
                            where c.AppUser.Id == user
                            orderby c.StockPortfolioNumber
                            select c;

                //execute query to store in list
                List<StockPortfolio> all = query.ToList();

                //convert list to select list format needed for HTML
                SelectList alllist = new SelectList(all, "StockPortfolioID", "StockPortfolioNumber");
                return alllist;
            }

        }
        //Get: Change Account Name
        public ActionResult ChangeAccountName(string SelectedType, int id, int AccountNumber, string AccountName)
        {
            return View();
        }


        //Post: Change Account Name
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAccountName(string SelectedType, int id,  string AccountName)
        {
            if (ModelState.IsValid)
            {
                if (SelectedType == "Checking")
                {
                    Checking accountToChange = db.Checkings.Find(id);
                    accountToChange.CheckingName = AccountName;
                    db.Entry(accountToChange).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (SelectedType == "Savings")
                {
                    Savings accountToChange = db.Savings.Find(id);
                    accountToChange.SavingsName = AccountName;
                    db.Entry(accountToChange).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (SelectedType == "IRA")
                {
                    IRA accountToChange = db.IRAs.Find(id);
                    accountToChange.IRAName = AccountName;
                    db.Entry(accountToChange).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (SelectedType == "Stock")
                {
                    StockPortfolio accountToChange = db.StockPortfolios.Find(id);
                    accountToChange.StockPortfolioName = AccountName;
                    db.Entry(accountToChange).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }

}