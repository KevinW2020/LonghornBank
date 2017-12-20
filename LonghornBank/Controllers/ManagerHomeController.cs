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

namespace LonghornBank.Controllers
{
    public class ManagerHomeController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: ManagerHome
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: ManagerHome
        [Authorize(Roles = "Manager")]
        public ActionResult CanBuy()
        {
            //limit to a particular account #
            var query = from s in db.StockPortfolios
                        where s.CanBuy != true
                        select s;

            List<StockPortfolio> AllStock = query.ToList();
            ViewBag.AllStock = AllStock;
            return View(ViewBag.AllStock);
        }

        // POST: ManagerHome
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult CanBuy(int SelectedID)
        {
            StockPortfolio stockToChange = db.StockPortfolios.Find(SelectedID);
            stockToChange.CanBuy = true;
            db.Entry(stockToChange).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("CanBuy");
        }

        // GET: ManagerHome
        [Authorize(Roles = "Manager")]
        public ActionResult Balanced()
        {
            //limit to a particular account #
            var query = from s in db.StockPortfolios
                        where s.BalanceStatus == "Balanced"
                        select s;

            List<StockPortfolio> AllStock = query.ToList();
            ViewBag.AllStock = AllStock;
            return View(ViewBag.AllStock);
        }

        // POST: ManagerHome
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Balanced(string ApplyBonus)
        {
            if(ApplyBonus == "Yes")
            {
                //limit to a particular account #
                var query = from s in db.StockPortfolios
                            where s.BalanceStatus == "Balanced"
                            select s;

                List<StockPortfolio> AllStock = query.ToList();

                foreach (StockPortfolio item in AllStock)
                {
                    item.CashBalance += (item.StockBalance * 0.1m);
                    db.Entry(item).State = EntityState.Modified;
                    Transaction transactionToChange = new Transaction();

                    transactionToChange.TypeOfTransaction = TransactionType.Bonus;
                    transactionToChange.TransactionAmount = Convert.ToDecimal(item.StockBalance * 0.1m);
                    transactionToChange.TransactionDescription = "Balanced Portfolio Bonus";
                    transactionToChange.TransactionDate = DateTime.Today;
                    transactionToChange.AccountReceiver = item.StockPortfolioNumber;
                    transactionToChange.AccountReceiverID = item.StockPortfolioID;
                    transactionToChange.AppUser = item.AppUser;
                    db.Transactions.Add(transactionToChange);
                    db.SaveChanges();
                }
                Success(string.Format("Bonuses successfully Applied"), true);
                return RedirectToAction("Balanced");
            }
            return RedirectToAction("Balanced");
        }

        // GET: ManagerHome
        [Authorize(Roles = "Manager")]
        public ActionResult AccountStatus()
        {
            ViewBag.AllAccounts = GetPossibleAccounts();
            return View();
        }

        // POST: ManagerHome
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult AccountStatus(string SelectedAccount, Enabled Status)
        {

            List<string> parsed = SelectedAccount.Split(',').ToList<string>();


            string SelectedAccountType = parsed[0];
            int Id = Convert.ToInt32(parsed[3]);
            int AccountNumber = Convert.ToInt32(parsed[1]);

            if(SelectedAccountType == "Checking")
            {
                Checking accountToChange = db.Checkings.Find(Id);
                accountToChange.Enableds = Status;
                db.Entry(accountToChange).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (SelectedAccountType == "Savings")
            {
                Savings accountToChange = db.Savings.Find(Id);
                accountToChange.Enableds = Status;
                db.Entry(accountToChange).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (SelectedAccountType == "IRA")
            {
                IRA accountToChange = db.IRAs.Find(Id);
                accountToChange.Enableds = Status;
                db.Entry(accountToChange).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                StockPortfolio accountToChange = db.StockPortfolios.Find(Id);
                accountToChange.Enableds = Status;
                db.Entry(accountToChange).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToAction("AccountStatus");
        }


        public SelectList GetPossibleAccounts()
        {

            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> Selected = new List<string>();
            var query = (from s in db.Checkings
                         select new { value1 = s.CheckingNumber, value2 = s.CheckingBalance, value3 = s.CheckingID });
            var query2 = (from s in db.Savings
                          select new { value1 = s.SavingsNumber, value2 = s.SavingsBalance, value3 = s.SavingsID });
            var query3 = (from s in db.IRAs
                          select new { value1 = s.IRANumber, value2 = s.IRABalance, value3 = s.IRAID });
            var query4 = (from s in db.StockPortfolios
                          select new { value1 = s.StockPortfolioNumber, value2 = s.CashBalance, value3 = s.StockPortfolioID });

            foreach (var item in query)
            {
                Selected.Add("Checking" + "," + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query2)
            {
                Selected.Add("Savings" + "," + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query3)
            {
                Selected.Add("IRA" + "," + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query4)
            {
                Selected.Add("Stock" + "," + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }

            //convert list to select list format needed for HTML
            SelectList selectedBankingType = new SelectList(Selected);
            return selectedBankingType;
        }
    }
}