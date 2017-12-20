using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Controllers;
using LonghornBank.Models;
using Microsoft.AspNet.Identity;

namespace LonghornBank.Controllers
{
    public class ApplyController : BaseController
    {
        private AppDbContext db = new AppDbContext();


        // GET: Apply/Index
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {

            ViewBag.BankingTypes = GetPossibleAccounts();
            return View();
        }

        // POST: Apply/Index
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string SelectedType, string StartingBalance, string SelectedName, TransactionStatus? ApprovalStatus)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            var iraquery = (from c in db.IRAs
                           select c.IRANumber).Max();
            var stockquery = (from c in db.StockPortfolios
                            select c.StockPortfolioNumber).Max();
            var checkingquery = (from c in db.Checkings
                            select c.CheckingNumber).Max();
            var savingsquery = (from c in db.Savings
                            select c.SavingsNumber).Max();


            int latest = (Math.Max(iraquery, Math.Max(stockquery, Math.Max(checkingquery, savingsquery))));

            if (SelectedType == "Checking")
            {
                Checking newcheck = new Checking();
                newcheck.AppUser = u;
                if (Convert.ToDecimal(StartingBalance) > 5000)
                {
                    newcheck.CheckingName = "Longhorn Bank Checking";
                    newcheck.CheckingBalance = 0.0m;
                    Pending pendingToChange = new Pending();
                    pendingToChange.AccountNumber = latest + 1;
                    pendingToChange.AccountType = SelectedType;
                    pendingToChange.AppUser = u;
                    pendingToChange.PendingStatus = PendingStatus.Submitted;
                    pendingToChange.Amount = Convert.ToDecimal(Convert.ToDecimal(StartingBalance));
                    db.Pendings.Add(pendingToChange);
                }
                else
                {
                    newcheck.CheckingBalance = Convert.ToDecimal(StartingBalance);
                }
                newcheck.CheckingNumber = Convert.ToInt32(latest + 1);
                if (SelectedName != null && SelectedName != "")
                {
                    newcheck.CheckingName = SelectedName;
                }
                else
                {
                    newcheck.CheckingName = "Longhorn Bank Checking";
                }
                db.Checkings.Add(newcheck);
                db.SaveChanges();

                //Create a transaction
                Transaction transactionToChange = new Transaction();
                transactionToChange.AppUser = u;
                transactionToChange.TransactionAmount = Convert.ToDecimal(StartingBalance);
                transactionToChange.TransactionDescription = "Initial Deposit";
                // TODO: Not today, should be user input
                transactionToChange.TransactionDate = DateTime.Today;
                transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                transactionToChange.AccountSender = newcheck.CheckingNumber;

                if (Convert.ToDecimal(StartingBalance) > 5000.0m)
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                }
                else
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                }

                db.Transactions.Add(transactionToChange);
                db.SaveChanges();

                Success(string.Format("Successfully applied for account."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }
            else if (SelectedType == "Savings")
            {
                Savings newsav = new Savings();
                newsav.AppUser = u;
                if (Convert.ToDecimal(StartingBalance) > 5000)
                {
                    newsav.SavingsName = "Longhorn Bank Savings";
                    newsav.SavingsBalance = 0.0m;
                    Pending pendingToChange = new Pending();
                    pendingToChange.AccountNumber = latest + 1;
                    pendingToChange.AccountType = SelectedType;
                    pendingToChange.AppUser = u;
                    pendingToChange.PendingStatus = PendingStatus.Submitted;
                    pendingToChange.Amount = Convert.ToDecimal(Convert.ToDecimal(StartingBalance));
                    db.Pendings.Add(pendingToChange);
                }
                else
                {
                    newsav.SavingsBalance = Convert.ToDecimal(StartingBalance);
                }
                newsav.SavingsNumber = Convert.ToInt32(latest + 1);
                if (SelectedName != null && SelectedName != "")
                {
                    newsav.SavingsName = SelectedName;
                }
                else
                {
                    newsav.SavingsName = "Longhorn Bank Savings";
                }
                db.Savings.Add(newsav);
                db.SaveChanges();

                //Create a transaction
                Transaction transactionToChange = new Transaction();
                transactionToChange.AppUser = u;
                transactionToChange.TransactionAmount = Convert.ToDecimal(StartingBalance);
                transactionToChange.TransactionDescription = "Initial Deposit";
                // TODO: Not today, should be user input
                transactionToChange.TransactionDate = DateTime.Today;
                transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                transactionToChange.AccountSender = newsav.SavingsNumber;

                if (Convert.ToDecimal(StartingBalance) > 5000.0m)
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                }
                else
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                }

                db.Transactions.Add(transactionToChange);
                db.SaveChanges();

                Success(string.Format("Successfully applied for account."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }
            else if (SelectedType == "IRA")
            {
                if (Convert.ToDecimal(StartingBalance) != 0 && (DateTime.Today-u.DOB).Days > 25567.5m)
                {
                    return RedirectToAction("ContributionAge");
                }
                if (Convert.ToDecimal(StartingBalance) > 5000)
                {
                    return RedirectToAction("ContributionLimit", new { StartingBalance = "5000"});
                }
                else
                {
                    IRA newira = new IRA();
                    newira.AppUser = u;
                    newira.IRAName = "Longhorn Bank IRA";
                    newira.IRABalance = Convert.ToDecimal(StartingBalance);
                    newira.Contribution = Convert.ToDecimal(StartingBalance);
                    newira.IRANumber = Convert.ToInt32(latest + 1);
                    if (SelectedName != null && SelectedName != "")
                    {
                        newira.IRAName = SelectedName;
                    }
                    else
                    {
                        newira.IRAName = "Longhorn Bank IRA";
                    }
                    db.IRAs.Add(newira);

                    //Create a transaction
                    Transaction transactionToChange = new Transaction();
                    transactionToChange.AppUser = u;
                    transactionToChange.TransactionAmount = Convert.ToDecimal(StartingBalance);
                    transactionToChange.TransactionDescription = "Initial Deposit";
                    // TODO: Not today, should be user input
                    transactionToChange.TransactionDate = DateTime.Today;
                    transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                    transactionToChange.AccountSender = newira.IRANumber;

                    if (Convert.ToDecimal(StartingBalance) > 5000.0m)
                    {
                        transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                    }
                    else
                    {
                        transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                    }

                    db.Transactions.Add(transactionToChange);
                    db.SaveChanges();

                    db.SaveChanges();
                }
                Success(string.Format("Successfully applied for account."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }
            else if (SelectedType == "Stock")
            {
                StockPortfolio  newstock = new StockPortfolio();
                newstock.AppUser = u;
                newstock.TotalFees = 0;
                if (Convert.ToDecimal(StartingBalance) > 5000)
                {
                    newstock.StockPortfolioName = "Longhorn Bank Stock Portfolio";
                    newstock.CashBalance = 0.0m;
                    Pending pendingToChange = new Pending();
                    pendingToChange.AccountNumber = latest + 1;
                    pendingToChange.AccountType = SelectedType;
                    pendingToChange.AppUser = u;
                    pendingToChange.PendingStatus = PendingStatus.Submitted;
                    pendingToChange.Amount = Convert.ToDecimal(Convert.ToDecimal(StartingBalance));
                    db.Pendings.Add(pendingToChange);
                }
                else
                {
                    newstock.CashBalance = Convert.ToDecimal(StartingBalance);
                }
                newstock.StockPortfolioNumber = Convert.ToInt32(latest + 1);
                if (SelectedName != null && SelectedName != "")
                {
                    newstock.StockPortfolioName = SelectedName;
                }
                else
                {
                    newstock.StockPortfolioName = "Longhorn Bank Stock Portfolio";
                }
                newstock.TotalFees = 0m;
                newstock.StockBalance = 0m;
                newstock.BalanceStatus = "UnBalanced";
                newstock.CanBuy = false;
                db.StockPortfolios.Add(newstock);
                db.SaveChanges();

                //Create a transaction
                Transaction transactionToChange = new Transaction();
                transactionToChange.AppUser = u;
                transactionToChange.TransactionAmount = Convert.ToDecimal(StartingBalance);
                transactionToChange.TransactionDescription = "Initial Deposit";
                // TODO: Not today, should be user input
                transactionToChange.TransactionDate = DateTime.Today;
                transactionToChange.TypeOfTransaction = TransactionType.Deposit;
                transactionToChange.AccountSender = newstock.StockPortfolioNumber;

                if (Convert.ToDecimal(StartingBalance) > 5000.0m)
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Pending;
                }
                else
                {
                    transactionToChange.ApprovalStatus = TransactionStatus.Approved;
                }

                db.Transactions.Add(transactionToChange);
                db.SaveChanges();

                Success(string.Format("Successfully applied for account."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }
            else
            {
                return RedirectToAction("Index", "ManageAccounts");
            }
        }
        public ActionResult ContributionLimit(string StartingBalance)
        {

            ViewBag.BankingTypes = IRASelectList();
            return View((object)StartingBalance);
        }

        // POST: Apply/Index
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult ContributionLimit(string SelectedType, string StartingBalance, string SelectedName)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            var iraquery = (from c in db.IRAs
                            select c.IRANumber).Max();
            var stockquery = (from c in db.StockPortfolios
                              select c.StockPortfolioNumber).Max();
            var checkingquery = (from c in db.Checkings
                                 select c.CheckingNumber).Max();
            var savingsquery = (from c in db.Savings
                                select c.SavingsNumber).Max();


            int latest = (Math.Max(iraquery, Math.Max(stockquery, Math.Max(checkingquery, savingsquery))));

            if (Convert.ToDecimal(StartingBalance) > 5000)
            {
                return RedirectToAction("ContributionLimit", new { StartingBalance = "5000", SelectedName });
            }
            else
            {
                IRA newira = new IRA();
                newira.AppUser = u;
                newira.IRABalance = Convert.ToDecimal(StartingBalance);
                newira.Contribution = Convert.ToDecimal(StartingBalance);
                newira.IRANumber = Convert.ToInt32(latest + 1);
                if (SelectedName != null && SelectedName != "")
                {
                    newira.IRAName = SelectedName;
                }
                else
                {
                    newira.IRAName = "Longhorn IRA";
                }
                db.IRAs.Add(newira);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "ManageAccounts");
        }
        private object Max(int iRANumber)
        {
            throw new NotImplementedException();
        }

        public SelectList GetPossibleAccounts()
        {

            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);



            List<string> newlist = new List<string>();
            newlist.Add("Savings");
            newlist.Add("Checking");


            if (u.IRAs.Count == 0 && u.StockPortfolios.Count == 0)
            {
                newlist.Add("IRA");
                newlist.Add("Stock");
            }
            else if (u.IRAs.Count == 0 && u.StockPortfolios.Count == 1)
            {
                newlist.Add("IRA");
            }
            else if (u.IRAs.Count == 1 && u.StockPortfolios.Count == 0)
            {
                newlist.Add("Stock");
            }


            //convert list to select list format needed for HTML
            SelectList selectedBankingType = new SelectList(newlist);
            return selectedBankingType;

        }

        public ActionResult ContributionAge()
        {
            return View();
        }

        public SelectList IRASelectList()
        {

            List<string> newlist = new List<string>();
            newlist.Add("IRA");

            //convert list to select list format needed for HTML
            SelectList selectedBankingType = new SelectList(newlist);
            return selectedBankingType;

        }
    }

}