using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Models;

namespace LonghornBank.Controllers
{
    public class DisputesController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: Disputes
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult Index()
        {
            var disputes = db.Disputes.Include(d => d.Transaction);
            return View(disputes.ToList());
        }

        // GET: Disputes/Details/5
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult Details(int? id)
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
            return View(dispute);
        }

        // GET: Disputes/Create
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Disputes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisputeStatus,Comments,CorrectAmount,DeleteTransaction")] Dispute dispute,Int32 id)
        {
            if (ModelState.IsValid)
            {
                dispute.Transaction = db.Transactions.Single(t => t.TransactionID == id);
                dispute.DisputeStatus = 0;
                db.Disputes.Add(dispute);
                db.SaveChanges();
                Success(string.Format("Dispute successfully created."), true);
                return RedirectToAction("Index", "ManageAccounts");
            }

            ViewBag.AllTransactions = new SelectList(db.Transactions.ToList(), "TransactionID", "TransactionDescription");
            return View(dispute);
        }

        // GET: Disputes/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.DisputeID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", dispute.DisputeID);
            return View(dispute);
        }

        // POST: Disputes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DisputeID,Comments,CorrectAmount,DeleteTransaction")] Dispute dispute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dispute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DisputeID = new SelectList(db.Transactions, "TransactionID", "TransactionDescription", dispute.DisputeID);
            return View(dispute);
        }

        // GET: Disputes/Delete/5
        public ActionResult Delete(int? id)
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
            return View(dispute);
        }

        // POST: Disputes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dispute dispute = db.Disputes.Find(id);
            db.Disputes.Remove(dispute);
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
    }
}
