using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Controllers;
using LonghornBank.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;

namespace LonghornBank.Controllers
{
    public class DepositController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: Deposit/Index
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            ViewBag.AllAccounts = GetPossibleAccounts();
            return View();

        }
        // POST: Deposit/Index
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string SelectedAccount, string SelectedAmount, string TransactionDate)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> parsed = SelectedAccount.Split(',').ToList<string>();

            Console.WriteLine(parsed);

            string SelectedAccountType = parsed[0];
            int Id = Convert.ToInt32(parsed[3]);
            int AccountNumber = Convert.ToInt32(parsed[1]);
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
                        return RedirectToAction("Index", "ManageAccounts");
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
                        return RedirectToAction("Index", "ManageAccounts");
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
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange.Contribution), SelectedAccount });
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
                        return RedirectToAction("Index", "ManageAccounts");
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
                        return RedirectToAction("Index", "ManageAccounts");
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
        public ActionResult DepositLimit(string SelectedAccount, string SelectedAmount)
        {
            ViewBag.SelectedAccount = SelectedAccount;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/DepositLimit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepositLimit(string TransactionDate, string SelectedAccount, string SelectedAmount)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> parsed = SelectedAccount.Split(',').ToList<string>();

            Console.WriteLine(parsed);

            string SelectedAccountType = parsed[0];
            int Id = Convert.ToInt32(parsed[3]);
            int AccountNumber = Convert.ToInt32(parsed[1]);

            if (ModelState.IsValid)
            {

                //Find associated account
                IRA accountToChange = db.IRAs.Find(Id);
                if ((accountToChange.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                {
                    return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange.Contribution), SelectedAccount });
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

        public SelectList GetPossibleAccounts()
        {

            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> Selected = new List<string>();
            var query = (from s in db.Checkings
                         where s.AppUser.Id == user && s.Enableds != Enabled.No
                         select new { value1 = s.CheckingNumber, value2 = s.CheckingBalance, value3 = s.CheckingID });
            var query2 = (from s in db.Savings
                          where s.AppUser.Id == user && s.Enableds != Enabled.No
                          select new { value1 = s.SavingsNumber, value2 = s.SavingsBalance, value3 = s.SavingsID });
            var query3 = (from s in db.IRAs
                          where s.AppUser.Id == user && s.Enableds != Enabled.No
                          select new { value1 = s.IRANumber, value2 = s.IRABalance, value3 = s.IRAID });
            var query4 = (from s in db.StockPortfolios
                          where s.AppUser.Id == user && s.Enableds != Enabled.No
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