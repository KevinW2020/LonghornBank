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
    public class PendingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Pendings
        public ActionResult Index()
        {
            return View(db.Pendings.ToList());
        }
        // GET: Pendings
        public ActionResult MyPending()
        {
            string user = User.Identity.GetUserId();

            //limit to a particular account #
            var query = from p in db.Pendings
                           select p;

            query = query.Where(p => p.AppUser.Id == user);

            List<Pending> AllPendings = query.ToList();

            return View(AllPendings);
        }

        // GET: Pendings/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pending pending = db.Pendings.Find(id);
            if (pending == null)
            {
                return HttpNotFound();
            }
            return View(pending);
        }

        // GET: Pendings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pendings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PendingID,PendingStatus,AccountType,AccountNumber,CorrectAmount")] Pending pending)
        {
            if (ModelState.IsValid)
            {
                db.Pendings.Add(pending);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pending);
        }

        // GET: Pendings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pending pending = db.Pendings.Find(id);
            if (pending == null)
            {
                return HttpNotFound();
            }
            return View(pending);
        }

        // POST: Pendings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PendingID,PendingStatus,AccountType,AccountNumber,CorrectAmount")] Pending pending)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pending).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pending);
        }

        // GET: Pendings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pending pending = db.Pendings.Find(id);
            if (pending == null)
            {
                return HttpNotFound();
            }
            return View(pending);
        }

        // POST: Pendings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pending pending = db.Pendings.Find(id);
            db.Pendings.Remove(pending);
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
