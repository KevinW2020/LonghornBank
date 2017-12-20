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
    public class TransferController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: Transfer/Index
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            ViewBag.Accounts = GetPossibleAccounts();
            return View();
        }

        //GET: Transfer/Index
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string SelectedFrom, string SelectedTo, string SelectedAmount, string TransactionDate)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> parsed1 = SelectedFrom.Split(',').ToList<string>();
            List<string> parsed2 = SelectedTo.Split(',').ToList<string>();

            string SelectedAccountType1 = parsed1[0];
            int Id1 = Convert.ToInt32(parsed1[3]);

            string SelectedAccountType2 = parsed2[0];
            int Id2 = Convert.ToInt32(parsed2[3]);

            if (SelectedAccountType1 == "Checking")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Checking accountToChange1 = db.Checkings.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.CheckingBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        if(SelectedAccountType2 == "Checking")
                        {

                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.CheckingNumber, accountToChange1.CheckingBalance);

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.CheckingNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.CheckingNumber, accountToChange1.CheckingBalance);
                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.CheckingNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA          
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate = TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.CheckingNumber, accountToChange1.CheckingBalance);
                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.CheckingNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.CheckingNumber, accountToChange1.CheckingBalance);
                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.CheckingNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {

                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                    }
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else if (SelectedAccountType1 == "Savings")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Savings accountToChange1 = db.Savings.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.SavingsBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.SavingsNumber, accountToChange1.SavingsBalance);

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.SavingsNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.SavingsNumber, accountToChange1.SavingsBalance);


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.SavingsNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.SavingsNumber, accountToChange1.SavingsBalance);


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.SavingsNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.SavingsNumber, accountToChange1.SavingsBalance);

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.SavingsNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                    }
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else if (SelectedAccountType1 == "IRA")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    IRA accountToChange1 = db.IRAs.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.IRABalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if ((Convert.ToDateTime(TransactionDate) - u.DOB).Days <= 23741.25m)
                    {
                        if (Convert.ToDecimal(SelectedAmount) > 3000)
                        {
                            return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                        }
                        else
                        {
                            return RedirectToAction("WithdrawLimit", new { SelectedAmount, SelectedTo, SelectedFrom });
                        }
                    }
                    if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, accountToChange1.IRABalance);


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, accountToChange1.IRABalance);

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, accountToChange1.IRABalance);


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, accountToChange1.IRABalance);


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                    }
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else if (SelectedAccountType1 == "Stock")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    StockPortfolio accountToChange1 = db.StockPortfolios.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.CashBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.StockPortfolioNumber, Convert.ToDecimal(accountToChange1.CashBalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.StockPortfolioNumber, Convert.ToDecimal(accountToChange1.CashBalance));


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.StockPortfolioNumber, Convert.ToDecimal(accountToChange1.CashBalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.StockPortfolioNumber, Convert.ToDecimal(accountToChange1.CashBalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange = new Transaction();
                            feeToChange.AccountSenderID = Id1;
                            feeToChange.AppUser = accountToChange1.AppUser;
                            feeToChange.TransactionAmount = 30;
                            feeToChange.TransactionDescription = "Overdraft fee";
                            // TODO: Not today, should be user input
                            feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange.TypeOfTransaction = TransactionType.Fee;
                            feeToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            db.Transactions.Add(feeToChange);

                            db.SaveChanges();
                        }

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        if (SelectedAccountType2 == "Checking")
                        {
                            //Find associated account
                            Checking accountToChange2 = db.Checkings.Find(Id2);
                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Savings")
                        {
                            //Find associated account
                            Savings accountToChange2 = db.Savings.Find(Id2);
                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);


                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "IRA")
                        {
                            //TODO: add the validation for IRA
                            //Find associated account
                            IRA accountToChange2 = db.IRAs.Find(Id2);
                            if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                            {
                                return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                            }
                            else
                            {
                                accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                                accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                            }
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                        else if (SelectedAccountType2 == "Stock")
                        {
                            //Find associated account
                            StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount));
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            db.SaveChanges();
                        }
                    }
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
                else
                {
                    Success(string.Format("Transfer completed successfully."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            else
            {
                Success(string.Format("Transfer completed successfully."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }
        }


        // GET: ManageAccounts/DepositLimit
        [Authorize(Roles = "Customer")]
        public ActionResult DepositLimit(string SelectedAmount, string SelectedFrom, string SelectedTo)
        {
            ViewBag.SelectedFrom = SelectedFrom;
            ViewBag.SelectedTo = SelectedTo;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/DepositLimit
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult DepositLimit(string TransactionDate, string SelectedAmount, string SelectedFrom, string SelectedTo)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> parsed1 = SelectedFrom.Split(',').ToList<string>();
            List<string> parsed2 = SelectedTo.Split(',').ToList<string>();

            string SelectedAccountType1 = parsed1[0];
            int Id1 = Convert.ToInt32(parsed1[3]);

            string SelectedAccountType2 = parsed2[0];
            int Id2 = Convert.ToInt32(parsed2[3]);

            if (SelectedAccountType1 == "Checking")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Checking accountToChange1 = db.Checkings.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.CheckingBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {

                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate = TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;
                        accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange1).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange1.CheckingNumber, Convert.ToDecimal(accountToChange1.CheckingBalance));


                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id1;
                        feeToChange.AppUser = accountToChange1.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = accountToChange1.CheckingNumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.CheckingBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        accountToChange1.CheckingBalance -= (Convert.ToDecimal(SelectedAmount));
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.CheckingNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.CheckingNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index", "ManageAccounts");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            if (SelectedAccountType1 == "Savings")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    Savings accountToChange1 = db.Savings.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.SavingsBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {

                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate = TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;
                        accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange1).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange1.SavingsNumber, Convert.ToDecimal(accountToChange1.SavingsBalance));


                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id1;
                        feeToChange.AppUser = accountToChange1.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = accountToChange1.SavingsNumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.SavingsBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        accountToChange1.SavingsBalance -= (Convert.ToDecimal(SelectedAmount));
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.SavingsNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.SavingsNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index", "ManageAccounts");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            if (SelectedAccountType1 == "IRA")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    IRA accountToChange1 = db.IRAs.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.IRABalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {

                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate = TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;
                        accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange1).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.IRANumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id1;
                        feeToChange.AppUser = accountToChange1.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = accountToChange1.IRANumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount));
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.IRANumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index", "ManageAccounts");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManageAccounts");
                }
            }
            if (SelectedAccountType1 == "Stock")
            {
                if (ModelState.IsValid)
                {
                    //Find associated account
                    StockPortfolio accountToChange1 = db.StockPortfolios.Find(Id1);
                    if (Convert.ToDecimal(SelectedAmount) <= 0)
                    {
                        return RedirectToAction("NegativeEntry");
                    }
                    if (accountToChange1.CashBalance < 0)
                    {
                        return RedirectToAction("NegativeBalance");
                    }
                    if (-50 <= (accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) < 0)
                    {

                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate = TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;
                        accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                        db.Entry(accountToChange1).State = EntityState.Modified;
                        EmailSender(u.Email, accountToChange1.StockPortfolioNumber, Convert.ToDecimal(accountToChange1.CashBalance));


                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        //Create a fee
                        Transaction feeToChange = new Transaction();
                        feeToChange.AccountSenderID = Id1;
                        feeToChange.AppUser = accountToChange1.AppUser;
                        feeToChange.TransactionAmount = 30;
                        feeToChange.TransactionDescription = "Overdraft fee";
                        // TODO: Not today, should be user input
                        feeToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        feeToChange.TypeOfTransaction = TransactionType.Fee;
                        feeToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                        db.Transactions.Add(feeToChange);

                        db.SaveChanges();

                        return RedirectToAction("Overdraft");
                    }
                    if ((accountToChange1.CashBalance - Convert.ToDecimal(SelectedAmount)) < -50)
                    {
                        return RedirectToAction("MaxOverdraft");
                    }
                    else
                    {
                        //TODO: add the validation for IRA
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if ((accountToChange2.Contribution + Convert.ToDecimal(SelectedAmount)) > 5000.0m)
                        {
                            return RedirectToAction("DepositLimit", new { SelectedAmount = (5000.0m - accountToChange2.Contribution), TransactionDate, SelectedFrom, SelectedTo });
                        }
                        else
                        {
                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            accountToChange2.Contribution += Convert.ToDecimal(SelectedAmount);
                        }
                        db.Entry(accountToChange2).State = EntityState.Modified;

                        accountToChange1.CashBalance -= (Convert.ToDecimal(SelectedAmount));
                        db.Entry(accountToChange1).State = EntityState.Modified;

                        //Create a transaction
                        Transaction transactionToChange = new Transaction();
                        transactionToChange.AccountSenderID = Id1;
                        transactionToChange.AccountReceiverID = Id2;
                        transactionToChange.AppUser = accountToChange1.AppUser;
                        transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                        transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.StockPortfolioNumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                        // TODO: Not today, should be user input
                        transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                        transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                        transactionToChange.AccountSender = accountToChange1.StockPortfolioNumber;
                        transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                        db.Transactions.Add(transactionToChange);

                        db.SaveChanges();
                        return RedirectToAction("Index", "ManageAccounts");
                    }
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


        // GET: ManageAccounts/WithdrawLimit
        [Authorize(Roles = "Customer")]
        public ActionResult WithdrawLimit(string SelectedAmount, string SelectedFrom, string SelectedTo)
        {
            ViewBag.SelectedFrom = SelectedFrom;
            ViewBag.SelectedTo = SelectedTo;
            return View((object)SelectedAmount);
        }

        // POST: ManageAccounts/WithdrawLimit
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult WithdrawLimit(string TransactionDate, string SelectedAmount, string SelectedFrom, string SelectedTo, string SelectedInclude)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);

            List<string> parsed1 = SelectedFrom.Split(',').ToList<string>();
            List<string> parsed2 = SelectedTo.Split(',').ToList<string>();

            string SelectedAccountType1 = parsed1[0];
            int Id1 = Convert.ToInt32(parsed1[3]);

            string SelectedAccountType2 = parsed2[0];
            int Id2 = Convert.ToInt32(parsed2[3]);


            if (ModelState.IsValid)
            {
                IRA accountToChange1 = db.IRAs.Find(Id1);
                if (SelectedInclude == "Yes")
                {
                    if (SelectedAccountType2 == "Checking")
                    {
                        //Find associated account
                        Checking accountToChange2 = db.Checkings.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.CheckingBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) - 30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.CheckingBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount)-30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "Savings")
                    {
                        //Find associated account
                        Savings accountToChange2 = db.Savings.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.SavingsBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) - 30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.SavingsBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) - 30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "IRA")
                    {
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.IRABalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) - 30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.IRABalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount)-30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "Stock")
                    {
                        //Find associated account
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.CashBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) - 30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.CashBalance += (Convert.ToDecimal(SelectedAmount)-30);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount) -30);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }

                    }
                    else
                    {
                        return RedirectToAction("Index", "ManageAccounts");
                    }
                }
                else
                {
                    if (SelectedAccountType2 == "Checking")
                    {
                        //Find associated account
                        Checking accountToChange2 = db.Checkings.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount));
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.CheckingBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount));
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.CheckingNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.CheckingNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "Savings")
                    {
                        //Find associated account
                        Savings accountToChange2 = db.Savings.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount));
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.SavingsBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.SavingsNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.SavingsNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "IRA")
                    {
                        //Find associated account
                        IRA accountToChange2 = db.IRAs.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount));
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.IRABalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.IRANumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.IRANumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }
                    }
                    else if (SelectedAccountType2 == "Stock")
                    {
                        //Find associated account
                        StockPortfolio accountToChange2 = db.StockPortfolios.Find(Id2);
                        if (Convert.ToDecimal(SelectedAmount) <= 0)
                        {
                            return RedirectToAction("NegativeEntry");
                        }
                        if (accountToChange1.IRABalance < 0)
                        {
                            return RedirectToAction("NegativeBalance");
                        }
                        if (-50 <= (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) && (accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < 0)
                        {

                            //TODO: add the validation for IRA
                            //Find associated account
                            if (Convert.ToDecimal(SelectedAmount) > 3000)
                            {
                                return RedirectToAction("WithdrawLimit", new { SelectedAmount = "3000", SelectedTo, SelectedFrom });
                            }

                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;
                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 60);
                            db.Entry(accountToChange1).State = EntityState.Modified;
                            EmailSender(u.Email, accountToChange1.IRANumber, Convert.ToDecimal(accountToChange1.IRABalance));


                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = (Convert.ToDecimal(SelectedAmount));
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange1 = new Transaction();
                            feeToChange1.AccountSenderID = Id1;
                            feeToChange1.AppUser = accountToChange1.AppUser;
                            feeToChange1.TransactionAmount = 30;
                            feeToChange1.TransactionDescription = "Overdraft fee";
                            db.Transactions.Add(feeToChange1);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                        }
                        if ((accountToChange1.IRABalance - Convert.ToDecimal(SelectedAmount)) < -50)
                        {
                            return RedirectToAction("MaxOverdraft");
                        }
                        else
                        {
                            //TODO: add the validation for IRA
                            //Find associated account

                            accountToChange2.CashBalance += Convert.ToDecimal(SelectedAmount);
                            db.Entry(accountToChange2).State = EntityState.Modified;

                            accountToChange1.IRABalance -= (Convert.ToDecimal(SelectedAmount) + 30);
                            db.Entry(accountToChange1).State = EntityState.Modified;

                            //Create a transaction
                            Transaction transactionToChange = new Transaction();
                            transactionToChange.AccountSenderID = Id1;
                            transactionToChange.AccountReceiverID = Id2;
                            transactionToChange.AppUser = accountToChange1.AppUser;
                            transactionToChange.TransactionAmount = Convert.ToDecimal(SelectedAmount);
                            transactionToChange.TransactionDescription = "Transfer from: " + accountToChange1.IRANumber.ToString() + " to: " + accountToChange2.StockPortfolioNumber.ToString();
                            // TODO: Not today, should be user input
                            transactionToChange.TransactionDate = Convert.ToDateTime(TransactionDate);
                            transactionToChange.TypeOfTransaction = TransactionType.Transfer;
                            transactionToChange.AccountSender = accountToChange1.IRANumber;
                            transactionToChange.AccountReceiver = accountToChange2.StockPortfolioNumber;
                            db.Transactions.Add(transactionToChange);

                            //Create a fee
                            Transaction feeToChange2 = new Transaction();
                            feeToChange2.AccountSenderID = Id1;
                            feeToChange2.AppUser = accountToChange1.AppUser;
                            feeToChange2.TransactionAmount = 30;
                            feeToChange2.TransactionDescription = "Un qualified dsitribution fee";
                            // TODO: Not today, should be user input
                            feeToChange2.TransactionDate = Convert.ToDateTime(TransactionDate);
                            feeToChange2.TypeOfTransaction = TransactionType.Fee;
                            feeToChange2.AccountSender = accountToChange1.IRANumber;
                            db.Transactions.Add(feeToChange2);

                            db.SaveChanges();
                            return RedirectToAction("Index", "ManageAccounts");
                        }

                    }
                    else
                    {
                        return RedirectToAction("Index", "ManageAccounts");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "ManageAccounts");
            }
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
                Selected.Add("Checking" + "," + "XXXXXX" + Convert.ToString(item.value1).Substring(6,4) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query2)
            {
                Selected.Add("Savings" + "," + "XXXXXX" + Convert.ToString(item.value1).Substring(6, 4) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query3)
            {
                Selected.Add("IRA" + "," + "XXXXXX" + Convert.ToString(item.value1).Substring(6, 4) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
            }
            foreach (var item in query4)
            {
                Selected.Add("Stock" + "," + "XXXXXX" + Convert.ToString(item.value1).Substring(6, 4) + "," + Convert.ToString(item.value2) + "," + Convert.ToString(item.value3));
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
            string body = "Account "+Convert.ToString(CheckingNumber)+" has incurred an overdraft fee of $30 because the balance has fallen below zero dollars and a recent transaction was done. The current balance for this account is"+Convert.ToString(CheckingBalance);

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