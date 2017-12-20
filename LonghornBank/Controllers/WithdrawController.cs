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
    public class WithdrawController : BaseController
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
                            return RedirectToAction("Index", "ManageAccounts");
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
                            return RedirectToAction("Index", "ManageAccounts");
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
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedAccount });
                            }
                            else
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount, SelectedAccount });
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
                                return RedirectToAction("Index", "ManageAccounts");

                            }
                            else
                            {
                                Danger(string.Format("You may not withdraw more than your account balance."), true);
                                return RedirectToAction("Index");
                            }
                        }
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
                            return RedirectToAction("Index", "ManageAccounts");
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
                    Danger(string.Format("Warning: The Deposit must be greater than 0."), true);
                    return RedirectToAction("Index");
                }
        }


        // GET: ManageAccounts/WithdrawLimit
        [Authorize(Roles = "Customer")]
        public ActionResult WithdrawLimit( string SelectedAmount, string SelectedAccount)
        {
            ViewBag.SelectedAccount = SelectedAccount;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/WithdrawLimit
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult WithdrawLimit(string TransactionDate, string SelectedAccount, string SelectedAmount, string SelectedInclude)
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

                if (Convert.ToDecimal(SelectedAmount) > 3000)
                {
                    return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedAccount });
                }
                else
                {
                    if (SelectedInclude == "Yes")
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
                return RedirectToAction("Index", "ManageAccounts");
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