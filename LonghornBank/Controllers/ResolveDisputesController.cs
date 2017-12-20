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
    public class ResolveDisputesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: ResolveDisputes
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var query = from d in db.Disputes
                        where d.DisputeStatus == DisputeStatus.Submitted
                        select d;

            List<Dispute> AllUnresolved = query.ToList();
            ViewBag.AllUnresolved = AllUnresolved;
            return View(ViewBag.AllUnresolved);
        }

        // GET: ResolveDisputes
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int SelectedDispute)
        {
            return RedirectToAction("Resolve", new { id = SelectedDispute});
        }
        // GET: ResolveDisputes
        [Authorize(Roles = "Manager")]
        public ActionResult Resolve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispute dispute = db.Disputes.Find(id);
            if (dispute == null)
            {
                return HttpNotFound();
            }
            ViewBag.SenderID = dispute.Transaction.AccountSenderID;
            ViewBag.Sender = dispute.Transaction.AccountSender;
            ViewBag.OldAmount = dispute.Transaction.TransactionAmount;
            ViewBag.DisputeID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", dispute.DisputeID);
            ViewBag.Email = dispute.Transaction.AppUser.Email;
            return View(dispute);
        }
        // POST: ResolveDisputes
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Resolve([Bind(Include = "DisputeID,Comments,CorrectAmount,DisputeStatus,DeleteTransaction,ManagerComments,Transaction")] Dispute dispute, Int32 SenderID, Int32 Sender, Decimal OldAmount, String Email )
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            if (ModelState.IsValid)
            {
                Dispute disputeToChange = db.Disputes.Find(dispute.DisputeID);
                disputeToChange.DisputeStatus = dispute.DisputeStatus;
                disputeToChange.DeleteTransaction = dispute.DeleteTransaction;
                Transaction transactionToChange = db.Transactions.Find(disputeToChange.Transaction.TransactionID);
                string UserEmail = transactionToChange.AppUser.Email;
                

                db.Entry(disputeToChange).State = EntityState.Modified;
                db.Entry(transactionToChange).State = EntityState.Modified;

                if (dispute.DisputeStatus == DisputeStatus.Submitted)
                {
                    return RedirectToAction("Index");
                }
                else if(dispute.DisputeStatus == DisputeStatus.Rejected)
                {
                    //send rejection email
                    string Approval = "rejected";
                    Decimal Amount = 0m;
                    transactionToChange.ManagerEmail = u.Email;
                    transactionToChange.ManagerComments = dispute.ManagerComments;
                    transactionToChange.TransactionDescription = "Dispute [" + dispute.DisputeStatus.ToString() + "]: " + transactionToChange.TransactionDescription;
                    EmailSender(UserEmail, dispute.ManagerComments, Approval, Amount, dispute.Transaction.AccountSender);
                }
                else
                {
                    if (disputeToChange.DeleteTransaction == true)
                    {
                        string Approval = "Approved";
                        transactionToChange.TransactionAmount = 0;
                        transactionToChange.ManagerEmail = u.Email;
                        transactionToChange.ManagerComments = dispute.ManagerComments;
                        transactionToChange.TransactionDescription = "Dispute [" + dispute.DisputeStatus.ToString() + "]: " + transactionToChange.TransactionDescription;
                        EmailSender(UserEmail, dispute.ManagerComments, Approval, dispute.CorrectAmount, dispute.Transaction.AccountSender);
                    }
                    else
                    {
                        string Approval = "Approved";
                        transactionToChange.TransactionAmount = dispute.CorrectAmount;
                        transactionToChange.ManagerEmail = u.Email;
                        transactionToChange.ManagerComments = dispute.ManagerComments;
                        transactionToChange.TransactionDescription = "Dispute [" + dispute.DisputeStatus.ToString() + "]: " + transactionToChange.TransactionDescription;
                        EmailSender(UserEmail, dispute.ManagerComments, Approval, dispute.CorrectAmount, dispute.Transaction.AccountSender);
                    }
                }

                db.SaveChanges();

                if (dispute.DisputeStatus == DisputeStatus.Accepted || dispute.DisputeStatus == DisputeStatus.Adjusted)
                {
                    if(disputeToChange.DeleteTransaction == true)
                    {
                        if (transactionToChange.TypeOfTransaction == TransactionType.Deposit)
                        {
                            Checking accountToChange1 = db.Checkings.Find(SenderID);
                            if (accountToChange1 != null && accountToChange1.CheckingNumber == Sender)
                            {
                                accountToChange1.CheckingBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange1).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            Savings accountToChange2 = db.Savings.Find(SenderID);
                            if (accountToChange2 != null && accountToChange2.SavingsNumber == Sender)
                            {
                                accountToChange2.SavingsBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            IRA accountToChange3 = db.IRAs.Find(SenderID);
                            if (accountToChange3 != null && accountToChange3.IRANumber == Sender)
                            {
                                accountToChange3.IRABalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange3).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            StockPortfolio accountToChange4 = db.StockPortfolios.Find(SenderID);
                            if (accountToChange4 != null && accountToChange4.StockPortfolioNumber == Sender)
                            {
                                accountToChange4.CashBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        else
                        {
                            Checking accountToChange1 = db.Checkings.Find(SenderID);
                            if (accountToChange1 != null && accountToChange1.CheckingNumber == Sender)
                            {
                                accountToChange1.CheckingBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange1).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            Savings accountToChange2 = db.Savings.Find(SenderID);
                            if (accountToChange2 != null && accountToChange2.SavingsNumber == Sender)
                            {
                                accountToChange2.SavingsBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            IRA accountToChange3 = db.IRAs.Find(SenderID);
                            if (accountToChange3 != null && accountToChange3.IRANumber == Sender)
                            {
                                accountToChange3.IRABalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange3).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            StockPortfolio accountToChange4 = db.StockPortfolios.Find(SenderID);
                            if (accountToChange4 != null && accountToChange4.StockPortfolioNumber == Sender)
                            {
                                accountToChange4.CashBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        if (transactionToChange.TypeOfTransaction == TransactionType.Deposit)
                        {
                            Checking accountToChange1 = db.Checkings.Find(SenderID);
                            if (accountToChange1 != null && accountToChange1.CheckingNumber == Sender)
                            {
                                accountToChange1.CheckingBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange1).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            Savings accountToChange2 = db.Savings.Find(SenderID);
                            if (accountToChange2 != null && accountToChange2.SavingsNumber == Sender)
                            {
                                accountToChange2.SavingsBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            IRA accountToChange3 = db.IRAs.Find(SenderID);
                            if (accountToChange3 != null && accountToChange3.IRANumber == Sender)
                            {
                                accountToChange3.IRABalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange3).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            StockPortfolio accountToChange4 = db.StockPortfolios.Find(SenderID);
                            if (accountToChange4 != null && accountToChange4.StockPortfolioNumber == Sender)
                            {
                                accountToChange4.CashBalance += (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        else
                        {
                            Checking accountToChange1 = db.Checkings.Find(SenderID);
                            if (accountToChange1 != null && accountToChange1.CheckingNumber == Sender)
                            {
                                accountToChange1.CheckingBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange1).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            Savings accountToChange2 = db.Savings.Find(SenderID);
                            if (accountToChange2 != null && accountToChange2.SavingsNumber == Sender)
                            {
                                accountToChange2.SavingsBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            IRA accountToChange3 = db.IRAs.Find(SenderID);
                            if (accountToChange3 != null && accountToChange3.IRANumber == Sender)
                            {
                                accountToChange3.IRABalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange3).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            StockPortfolio accountToChange4 = db.StockPortfolios.Find(SenderID);
                            if (accountToChange4 != null && accountToChange4.StockPortfolioNumber == Sender)
                            {
                                accountToChange4.CashBalance -= (disputeToChange.CorrectAmount - OldAmount);
                                db.Entry(accountToChange4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                }


                return RedirectToAction("Index");
            }
            ViewBag.DisputeID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", dispute.DisputeID);
            return View(dispute);
        }
        public static void EmailSender(string strEmail, string comments, string approval, decimal amount, Int32 Sender)
        {

            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "longhornbanking@gmail.com";
            string password = "longhornbanking";
            string emailTo = strEmail;
            string subject = "Team 5: Dispute update" ;
            string body = "";
            if (approval == "rejected")
            {
                body = "Your dispute  has been rejected. Manager Comment: " + comments;
            }
            else
            {
                body = "Your dispute for has been approved!"+System.Environment.NewLine+"Manager Comment: " + comments+"."+System.Environment.NewLine+"The transaction amount has been updated to "+Convert.ToString(amount);

            }
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