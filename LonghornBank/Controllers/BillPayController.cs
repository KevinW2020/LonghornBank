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
    public class BillPayController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: BillPay/Index
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            //var query = from p in db.Payees
            //            where p.AppUsers.Any(tag => tag.Id == user)
            //            select p;
            var query = from p in db.Payees
                        where p.AppUsers.Any(tag => tag.Id == user && tag.Id != null)
                        select p;

            List<Payee> PayeeList = query.ToList();
            ViewBag.Accounts = GetPossibleAccounts();
            //ViewBag.MyPayees = MyPayees();
            ViewBag.MineOnly = PayeeList;
            return View(ViewBag.MineOnly);
        }
        // POST: BillPay/Index
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Index( string SelectedAccount, string SelectedAmount, string TransactionDate, int SelectedPayee, string Description)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            Payee p = db.Payees.Find(SelectedPayee);

            List<string> parsed = SelectedAccount.Split(',').ToList<string>();

            Console.WriteLine(parsed);

            string SelectedAccountType = parsed[0];
            int Id = Convert.ToInt32(parsed[3]);
            int AccountNumber = Convert.ToInt32(parsed[1]);

            if (SelectedAccountType == "Checking")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Checking accountToChange = db.Checkings.Find(Id);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if(accountToChange.CheckingBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if(-50 <= (accountToChange.CheckingBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        accountToChange.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange.CheckingNumber, accountToChange.CheckingBalance);

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.Payee = p;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = Description;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.BillPay;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id;
                        feeToChange.Payee = p;
                        feeToChange.AppUser = accountToChange.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();
                        Success(string.Format("Bill successfully paid."), true);
                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        accountToChange.CheckingBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.Payee = p;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = Description;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.BillPay;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                    }
                    Success(string.Format("Bill successfully paid."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    Success(string.Format("Dispute successfully created."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else if (SelectedAccountType == "Savings")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Savings accountToChange = db.Savings.Find(Id);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange.SavingsBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange.SavingsBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        accountToChange.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange.SavingsNumber, accountToChange.SavingsBalance);

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.Payee = p;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = Description;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.BillPay;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id;
                        feeToChange.Payee = p;
                        feeToChange.AppUser = accountToChange.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();
                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        accountToChange.SavingsBalance -= Convert.ToDecimal(SelectedAmount);
                        db.Entry(accountToChange).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id;
                        transactionToChange.Payee = p;
                        transactionToChange.AppUser = accountToChange.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = Description;
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.BillPay;
                        transactionToChange.AccountSender = AccountNumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                    }
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManageAccounts");
            }
        }

        // GET: BillPay/AddPayee
        [Authorize(Roles = "Customer")]
        public ActionResult AddPayee()
        {
            ViewBag.AllThePayees = AllPayees();
            return View();
        }

        // GET: BillPay/AddPayee
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult AddPayee(int PayeeID)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            Payee p = db.Payees.Find(PayeeID);
            p.AppUsers.Add(u);
            u.Payees.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult NegativeEntry()
        {
            return View();
        }
        public ActionResult NegativeBalance()
        {
            return View();
        }
        public ActionResult Overdraft()
        {
            return View();
        }
        public ActionResult MaxOverdraft()
        {
            return View();
        }

        public SelectList MyPayees()
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            //limit to a particular account #
            var query = from p in db.Payees
                        where p.AppUsers.Any(tag => tag.Id == user)
                        select p;

            List<Payee> MyPayee = query.ToList();
            //convert list to select list format needed for HTML
            SelectList selectedPayees = new SelectList(MyPayee, "PayeeID", "Name");
            return selectedPayees;

        }

        public SelectList AllPayees()
        {
            //limit to a particular account #
            var query = from p in db.Payees
                        select p;

            List<Payee> AllPayee = query.ToList();
            //convert list to select list format needed for HTML
            SelectList selectedPayees = new SelectList(AllPayee, "PayeeID", "Name");
            return selectedPayees;

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
            
            foreach (var item in query)
            {
                Selected.Add("Checking" + ","  + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query2)
            {
                Selected.Add("Savings" + "," + Convert.ToString(item.value1) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }

            //convert list to select list format needed for HTML
            SelectList selectedBankingType = new SelectList(Selected);
            return selectedBankingType;
        }
        public static void EmailSender(String strEmail, Int32 CheckingNumber, Decimal CheckingBalance)
        {

            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "longhornbanking@gmail.com";
            string password = "longhornbanking";
            string emailTo = strEmail;
            string subject = "Team 5: Overdraft Notification for Account" + Convert.ToString(CheckingNumber);
            string body = "Account " + Convert.ToString(CheckingNumber) + " has incurred an overdraft fee of $30 because the balance has fallen below zero dollars and a recent transaction was done. The current balance for this account is" + Convert.ToString(CheckingBalance);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                //Don't forget to fix this, make it so that email actually sends a link!!!!!!
                mail.Body = body;
                mail.IsBodyHtml = true;


                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

    }
}