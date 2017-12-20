using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Models;

namespace LonghornBank.Controllers
{

    public enum SortOrder
    {
        AscTransactionNumber,
        DescTransactionNumber,
        AscTransactionType,
        DescTransactionType,
        AscTransactionDescription,
        DescTransactionDescription,
        AscTransactionAmount,
        DescTransactionAmount,
        AscTransactionDate,
        DescTransactionDate
    }

    public class TransactionController : Controller
    {
        public static AppDbContext db = new AppDbContext();

        [Authorize(Roles = "Employee, Manager")]
        public ActionResult Index(string SearchNum, string SearchString, int? daysBack, DateTime? startDate, DateTime? endDate, int? price, decimal? priceFloor, decimal? priceCap, TransactionType? SelectedType, TransactionStatus? ApprovalStatus, SortOrder? SelectedSortOrder)
        {

            if ((SearchNum == null || SearchNum == "") && (SearchString == null || SearchNum == "") && (daysBack == null) && (startDate == null) && (endDate == null) && (price == null) && (priceFloor == null) && (priceCap == null) && (SelectedType == null))
            {
                //ViewBag.NumberAllTransactions = db.Transactions.ToList().Count();
                //ViewBag.NumberSelectedTransactions = db.Transactions.ToList().Count();

                var queryA = from t in db.Transactions
                             select t;

                queryA = queryA.Where(t => t.ApprovalStatus == TransactionStatus.Approved);

                List<Transaction> ApprovedTransactions = queryA.ToList();

                ViewBag.NumberAllTransactions = db.Transactions.ToList().Count();
                ViewBag.NumberSelectedTransactions = ApprovedTransactions.Count;

                return View(ApprovedTransactions);

                //return View(db.Transactions);
            }

            var query = from t in db.Transactions
                        select t;

            // Search by number
            if (SearchNum == null || SearchNum == "")
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TransactionID.ToString().Contains(SearchNum));
            }

            // Search by name
            if (SearchString == null || SearchString == "")
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TransactionDescription.Contains(SearchString));
            }

            // Filter by date
            if (daysBack == 0)
            {
                query = query;
            }
            else
            {
                if (daysBack == 696969)
                {
                    query = query.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);
                }
                else
                {
                    var oldestDate = (DateTime.Today).AddDays(Convert.ToDouble(daysBack) * -1);
                    query = query.Where(t => t.TransactionDate >= oldestDate);
                }
            }

            // Filter by price
            if (price == 707070)
            {
                query = query;
            }
            else
            {
                if (price == 696969)
                {
                    query = query.Where(t => t.TransactionAmount >= priceFloor && t.TransactionAmount <= priceCap);
                }
                else
                {
                    if (price == 300)
                    {
                        query = query.Where(t => t.TransactionAmount >= price);
                    }
                    else
                    {
                        var rangeCap = price + 100;
                        query = query.Where(t => t.TransactionAmount >= price && t.TransactionAmount <= rangeCap);
                    }
                }
            }

            // Filter by type
            if (SelectedType == null)
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TypeOfTransaction == SelectedType);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionNumber)
            {
                query = query.OrderBy(t => t.TransactionID);
            }
            
            if (SelectedSortOrder == SortOrder.DescTransactionNumber)
            {
                query = query.OrderByDescending(t => t.TransactionID);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionType)
            {
                query = query.OrderBy(t => t.TypeOfTransaction.ToString());
            }

            if (SelectedSortOrder == SortOrder.DescTransactionType)
            {
                query = query.OrderByDescending(t => t.TypeOfTransaction.ToString());
            }

            if (SelectedSortOrder == SortOrder.AscTransactionDescription)
            {
                query = query.OrderBy(t => t.TransactionDescription);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionDescription)
            {
                query = query.OrderByDescending(t => t.TransactionDescription);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionAmount)
            {
                query = query.OrderBy(t => t.TransactionAmount);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionAmount)
            {
                query = query.OrderByDescending(t => t.TransactionAmount);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionDate)
            {
                query = query.OrderBy(t => t.TransactionDate);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionDate)
            {
                query = query.OrderByDescending(t => t.TransactionDate);
            }


            List<Transaction> SelectedTransactions = query.ToList();

            ViewBag.NumberAllTransactions = db.Transactions.ToList().Count();
            ViewBag.NumberSelectedTransactions = SelectedTransactions.Count;

            return View(SelectedTransactions);
        }

        [Authorize(Roles = "Manager, Employee")]
        public ActionResult Pending(string SearchNum, string SearchString, int? daysBack, DateTime? startDate, DateTime? endDate, int? price, decimal? priceFloor, decimal? priceCap, TransactionType? SelectedType, TransactionStatus? ApprovalStatus, SortOrder? SelectedSortOrder)
        {

            if ((SearchNum == null || SearchNum == "") && (SearchString == null || SearchNum == "") && (daysBack == null) && (startDate == null) && (endDate == null) && (price == null) && (priceFloor == null) && (priceCap == null) && (SelectedType == null))
            {

                //limit to a particular account #

                var queryAll = from t in db.Transactions
                             select t;

                queryAll = queryAll.Where(t => t.ApprovalStatus == TransactionStatus.Pending);

                List<Transaction> AllTransactions = queryAll.ToList();

                ViewBag.NumberAllTransactions = db.Transactions.ToList().Count();
                ViewBag.NumberSelectedTransactions = AllTransactions.Count;

                return View(AllTransactions);

                //return View(db.Transactions);
            }


            var query = from t in db.Transactions
                        select t;

            query = query.Where(t => t.ApprovalStatus == TransactionStatus.Pending);


            // Search by number
            if (SearchNum == null || SearchNum == "")
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TransactionID.ToString().Contains(SearchNum));
            }

            // Search by name
            if (SearchString == null || SearchString == "")
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TransactionDescription.Contains(SearchString));
            }

            // Filter by date
            if (daysBack == 0)
            {
                query = query;
            }
            else
            {
                if (daysBack == 696969)
                {
                    query = query.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);
                }
                else
                {
                    var oldestDate = (DateTime.Today).AddDays(Convert.ToDouble(daysBack) * -1);
                    query = query.Where(t => t.TransactionDate >= oldestDate);
                }
            }

            // Filter by price
            if (price == 707070)
            {
                query = query;
            }
            else
            {
                if (price == 696969)
                {
                    query = query.Where(t => t.TransactionAmount >= priceFloor && t.TransactionAmount <= priceCap);
                }
                else
                {
                    if (price == 300)
                    {
                        query = query.Where(t => t.TransactionAmount >= price);
                    }
                    else
                    {
                        var rangeCap = price + 100;
                        query = query.Where(t => t.TransactionAmount >= price && t.TransactionAmount <= rangeCap);
                    }
                }
            }

            // Filter by type
            if (SelectedType == null)
            {
                query = query;
            }
            else
            {
                query = query.Where(t => t.TypeOfTransaction == SelectedType);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionNumber)
            {
                query = query.OrderBy(t => t.TransactionID);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionNumber)
            {
                query = query.OrderByDescending(t => t.TransactionID);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionType)
            {
                query = query.OrderBy(t => t.TypeOfTransaction.ToString());
            }

            if (SelectedSortOrder == SortOrder.DescTransactionType)
            {
                query = query.OrderByDescending(t => t.TypeOfTransaction.ToString());
            }

            if (SelectedSortOrder == SortOrder.AscTransactionDescription)
            {
                query = query.OrderBy(t => t.TransactionDescription);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionDescription)
            {
                query = query.OrderByDescending(t => t.TransactionDescription);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionAmount)
            {
                query = query.OrderBy(t => t.TransactionAmount);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionAmount)
            {
                query = query.OrderByDescending(t => t.TransactionAmount);
            }

            if (SelectedSortOrder == SortOrder.AscTransactionDate)
            {
                query = query.OrderBy(t => t.TransactionDate);
            }

            if (SelectedSortOrder == SortOrder.DescTransactionDate)
            {
                query = query.OrderByDescending(t => t.TransactionDate);
            }


            List<Transaction> SelectedTransactions = query.ToList();

            ViewBag.NumberAllTransactions = db.Transactions.ToList().Count();
            ViewBag.NumberSelectedTransactions = SelectedTransactions.Count;

            return View(SelectedTransactions);
        }


        public SelectList GetPossibleTypes()
        {

            //create query to find all committees
            var depquery = from c in db.Transactions
                           where c.TypeOfTransaction.ToString() == "Deposit"
                           select c;

            var withquery = from c in db.Transactions
                           where c.TypeOfTransaction.ToString() == "Withdrawal"
                           select c;

            var trquery = from c in db.Transactions
                           where c.TypeOfTransaction.ToString() == "Transfer"
                           select c;


            List<string> newlist = new List<string>();
            newlist.Add("Deposit");
            newlist.Add("Withdrawal");
            newlist.Add("Transfer");


            //convert list to select list format needed for HTML
            SelectList selectedTransactionType = new SelectList(newlist);
            return selectedTransactionType;

        }

    }

}