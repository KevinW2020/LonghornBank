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
    public class StockListsController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: StockLists
        public ActionResult Index()
        {
            return View(db.StockLists.ToList());
        }

        // GET: StockLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockList stockList = db.StockLists.Find(id);
            if (stockList == null)
            {
                return HttpNotFound();
            }
            return View(stockList);
        }

        // GET: StockLists/Create
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockListID,Name,Ticker,StockType,TransactionFee")] StockList stockList)
        {
            if (ModelState.IsValid)
            {
                db.StockLists.Add(stockList);
                db.SaveChanges();
                Success(string.Format("Stock successfully created."), true);
                return RedirectToAction("Index");
            }

            return View(stockList);
        }

        // GET: StockLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockList stockList = db.StockLists.Find(id);
            if (stockList == null)
            {
                return HttpNotFound();
            }
            return View(stockList);
        }

        // POST: StockLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockListID,Name,Ticker,StockType,TransactionFee")] StockList stockList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockList);
        }

        // GET: StockLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockList stockList = db.StockLists.Find(id);
            if (stockList == null)
            {
                return HttpNotFound();
            }
            return View(stockList);
        }

        // POST: StockLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockList stockList = db.StockLists.Find(id);
            db.StockLists.Remove(stockList);
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
