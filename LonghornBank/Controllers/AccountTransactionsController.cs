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
using System.Net.Mail;

namespace LonghornBank.Controllers
{
    public class AccountTransactionsController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        [Authorize(Roles = "Customer, Manager, Employee")]
        // GET: AccountTransactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Disputes);
            return View(transactions.ToList());
        }

        [Authorize(Roles = "Customer, Manager, Employee")]
        // GET: AccountTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.DetailID = id;
            return View(transaction);
        }


        public ActionResult Similar(int? id)
        {
            string user = User.Identity.GetUserId();

            Transaction similarof = db.Transactions.Find(id);

            //limit to a particular account #
            var query = from t in db.Transactions
                        where t.TransactionID != id && t.AppUser.Id == user
                        select t;

            query = query.OrderByDescending(t => t.TypeOfTransaction == similarof.TypeOfTransaction).ThenByDescending(t => t.TransactionDate).Take(5);
            List<Transaction> AllTrans = query.ToList();
            ViewBag.AllTrans = AllTrans;
            return View(ViewBag.AllTrans);
        }


        // GET: AccountTransactions/Create
        public ActionResult Create()
        {
            ViewBag.TransactionID = new SelectList(db.Disputes, "DisputeID", "Comments");
            return View();
        }

        // POST: AccountTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,TypeOfTransaction,ApprovalStatus,TransactionDate,TransactionAmount,TransactionDescription,TransactionComments,DisputeStatus")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TransactionID = new SelectList(db.Disputes, "DisputeID", "Comments", transaction.TransactionID);
            return View(transaction);
        }

        // GET: AccountTransactions/Edit/5
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = transaction.AccountSenderID;
            ViewBag.UserEmail = transaction.AppUser.Email;
            ViewBag.TransactionID = new SelectList(db.Disputes, "DisputeID", "Comments", transaction.TransactionID);
            return View(transaction);
        }

        // POST: AccountTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionID,TypeOfTransaction,ApprovalStatus,TransactionDate,TransactionAmount,TransactionDescription,TransactionComments,DisputeStatus,AccountSender,AccountSenderID")] Transaction transaction, string TransactionDate, string SelectedAccountType, int? Id, int? AccountNumber, TransactionStatus? ApprovalStatus, Int32 AccountID, String UserEmail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                if (ApprovalStatus == TransactionStatus.Approved)
                {
                    Checking accountToChange1 = db.Checkings.Find(transaction.AccountSenderID);
                    if (accountToChange1 != null && accountToChange1.CheckingNumber == transaction.AccountSender)
                    {
                        accountToChange1.CheckingBalance += transaction.TransactionAmount;
                        db.Entry(accountToChange1).State = EntityState.Modified;
                        db.SaveChanges();
                        EmailSender(UserEmail, accountToChange1.CheckingNumber, transaction.TransactionAmount);

                    }

                    Savings accountToChange2 = db.Savings.Find(transaction.AccountSenderID);
                    if (accountToChange2 != null && accountToChange2.SavingsNumber == transaction.AccountSender)
                    {
                        accountToChange2.SavingsBalance += transaction.TransactionAmount;
                        db.Entry(accountToChange2).State = EntityState.Modified;
                        db.SaveChanges();
                        EmailSender(UserEmail, accountToChange2.SavingsNumber, accountToChange2.SavingsBalance);

                    }

                    IRA accountToChange3 = db.IRAs.Find(transaction.AccountSenderID);
                    if (accountToChange3 != null && accountToChange3.IRANumber == transaction.AccountSender)
                    {
                        accountToChange3.IRABalance += transaction.TransactionAmount;
                        db.Entry(accountToChange3).State = EntityState.Modified;
                        db.SaveChanges();
                        EmailSender(UserEmail, accountToChange3.IRANumber, accountToChange3.IRABalance);

                    }

                    StockPortfolio accountToChange4 = db.StockPortfolios.Find(transaction.AccountSenderID);
                    if (accountToChange4 != null && accountToChange4.StockPortfolioNumber == transaction.AccountSender)
                    {
                        accountToChange4.CashBalance += transaction.TransactionAmount;
                        db.Entry(accountToChange4).State = EntityState.Modified;
                        db.SaveChanges();
                        EmailSender(UserEmail, accountToChange4.StockPortfolioNumber, Convert.ToDecimal(accountToChange4.CashBalance));

                    }
                }


                return RedirectToAction("Index");
            }
            ViewBag.TransactionID = new SelectList(db.Disputes, "DisputeID", "Comments", transaction.TransactionID);
            return View(transaction);
        }


        // GET: AccountTransactions/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: AccountTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public static void EmailSender(String strEmail, int CheckingNumber, Decimal CheckingBalance)
        {

            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "longhornbanking@gmail.com";
            string password = "longhornbanking";
            string emailTo = strEmail;
            string subject = "Team 5: Deposit approval for account " + Convert.ToString(CheckingNumber);
            string body = "The deposit for account " + Convert.ToString(CheckingNumber) + " has been approved! " + Convert.ToString(CheckingBalance)+" has been added to the account.";

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
