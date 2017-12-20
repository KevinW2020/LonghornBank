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
    public class ResolvePendingController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: ResolvePending
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var query = from t in db.Transactions
                        where t.ApprovalStatus == TransactionStatus.Pending
                        select t;

            List<Transaction> AllUnresolved = query.ToList();
            ViewBag.AllUnresolved = AllUnresolved;
            return View(ViewBag.AllUnresolved);
        }

        // GET: ResolvePending
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int SelectedTransaction)
        {
            return RedirectToAction("Resolve", new { id = SelectedTransaction });
        }

        // GET: ResolveDisputes

        [Authorize(Roles = "Manager")]
        public ActionResult Resolve(int? id)
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
            ViewBag.TransactionID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", transaction.TransactionID);
            return View(transaction);
        }

        // POST: ResolveDisputes
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Resolve([Bind(Include = "TransactionID,TransactionDescription,TransactionAmount,ApprovalStatus,TransactionComments")] Transaction transaction)
        {
            string user = User.Identity.GetUserId();
            AppUser u = db.Users.Find(user);
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                if (transaction.ApprovalStatus == TransactionStatus.Pending)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Transaction transactionToChange = db.Transactions.Find(transaction.TransactionID);
                    transactionToChange.TransactionAmount = transaction.TransactionAmount;
                    transactionToChange.ManagerEmail = u.Email;
                    transactionToChange.ManagerComments = transaction.TransactionComments;
                    transactionToChange.ApprovalStatus = TransactionStatus.Approved;

                    //add the logic to search through and actually modify the account
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DisputeID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", transaction.TransactionID);
            return View(transaction);
        }


        //    // GET: ResolvePending/Details/5
        //    public ActionResult Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Transaction transaction = db.Transactions.Find(id);
        //        if (transaction == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(transaction);
        //    }

        //    // GET: ResolvePending/Create
        //    public ActionResult Create()
        //    {
        //        return View();
        //    }

        //    // POST: ResolvePending/Create
        //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Create([Bind(Include = "TransactionID,TypeOfTransaction,ApprovalStatus,TransactionDate,TransactionAmount,TransactionDescription,TransactionComments,ManagerComments,ManagerEmail,AccountSender,AccountReceiver")] Transaction transaction)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Transactions.Add(transaction);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //        return View(transaction);
        //    }

        //    // GET: ResolvePending/Edit/5
        //    public ActionResult Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Transaction transaction = db.Transactions.Find(id);
        //        if (transaction == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(transaction);
        //    }

        //    // POST: ResolvePending/Edit/5
        //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit([Bind(Include = "TransactionID,TypeOfTransaction,ApprovalStatus,TransactionDate,TransactionAmount,TransactionDescription,TransactionComments,ManagerComments,ManagerEmail,AccountSender,AccountReceiver")] Transaction transaction)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(transaction).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        return View(transaction);
        //    }

        //    // GET: ResolvePending/Delete/5
        //    public ActionResult Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Transaction transaction = db.Transactions.Find(id);
        //        if (transaction == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(transaction);
        //    }

        //    // POST: ResolvePending/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult DeleteConfirmed(int id)
        //    {
        //        Transaction transaction = db.Transactions.Find(id);
        //        db.Transactions.Remove(transaction);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            db.Dispose();
        //        }
        //        base.Dispose(disposing);
        //    }
    }
}
