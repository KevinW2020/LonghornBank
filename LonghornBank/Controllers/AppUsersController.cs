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
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;

namespace LonghornBank.Controllers
{
    public class UsersController : BaseController
    {
        private AppDbContext db = new AppDbContext();

        // GET: Users
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult EmployeeIndex()
        {
            DateTime date = Convert.ToDateTime("01/01/1990");
            var query = from u in db.Users
                        where u.DOB != date
                        select u;

            return View(query.ToList());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Manager, Employee, Customer")]
        public ActionResult Details()
        {
            string id = User.Identity.GetUserId();


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id != User.Identity.GetUserId<string>() && !User.IsInRole("Manager, Employee"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }


        //
        //
        //Get: Users/managedetails/5 
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult ManageDetails(string id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }



        // GET: Users/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LName,FName,Initial,ActiveUser,DOB,StreetAddress,City,State,ZIP,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Manager, Customer, Employee")]
        public ActionResult Edit()
        {

            string id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Customer, Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LName,FName,Initial,StreetAddress,City,State,ZIP,Email,PhoneNumber")] AppUser appUser)
        {

            string id = User.Identity.GetUserId();

            if (ModelState.IsValid)
                
            {
                if (User.IsInRole("Manager"))
                {

                    AppUser UserTobeChanged = db.Users.Find(appUser.Id);
                    UserTobeChanged.LName = appUser.LName;
                    UserTobeChanged.FName = appUser.FName;
                    UserTobeChanged.Initial = appUser.Initial;
                    UserTobeChanged.StreetAddress = appUser.StreetAddress;
                    UserTobeChanged.City = appUser.City;
                    UserTobeChanged.State = appUser.State;
                    UserTobeChanged.ZIP = appUser.ZIP;
                    UserTobeChanged.Email = appUser.Email;
                    UserTobeChanged.PhoneNumber = appUser.PhoneNumber;

                    db.Entry(UserTobeChanged).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details");
                }

                if (User.IsInRole("Customer"))
                {

                    AppUser UserTobeChanged = db.Users.Find(appUser.Id);
                    UserTobeChanged.LName = appUser.LName;
                    UserTobeChanged.FName = appUser.FName;
                    UserTobeChanged.Initial = appUser.Initial;
                    UserTobeChanged.StreetAddress = appUser.StreetAddress;
                    UserTobeChanged.City = appUser.City;
                    UserTobeChanged.State = appUser.State;
                    UserTobeChanged.ZIP = appUser.ZIP;
                    UserTobeChanged.Email = appUser.Email;
                    UserTobeChanged.PhoneNumber = appUser.PhoneNumber;

                    db.Entry(UserTobeChanged).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details");
                }

                if (User.IsInRole("Employee"))
                {
                    
                    AppUser UserTobeChanged = db.Users.Find(appUser.Id);
                    UserTobeChanged.StreetAddress = appUser.StreetAddress;
                    UserTobeChanged.City = appUser.City;
                    UserTobeChanged.State = appUser.State;
                    UserTobeChanged.ZIP = appUser.ZIP;
                    UserTobeChanged.PhoneNumber = appUser.PhoneNumber;
                  
                    db.Entry(UserTobeChanged).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    return RedirectToAction("Details");
                }

                
            }
            
            return View(appUser);
        }

       

        // GET: Users/ManageEdit/5
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult ManageEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: Users/ManageEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Employee")]
        [ValidateAntiForgeryToken]
        public ActionResult ManageEdit([Bind(Include = "Id,LName,FName,Initial,StreetAddress,City,State,ZIP,Email,PhoneNumber")] AppUser appUser)
        {
            string tempid = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Manager"))
                {
                    AppUser UserTobeChanged = db.Users.Find(appUser.Id);
                    UserTobeChanged.LName = appUser.LName;
                    UserTobeChanged.FName = appUser.FName;
                    UserTobeChanged.Initial = appUser.Initial;
                    UserTobeChanged.StreetAddress = appUser.StreetAddress;
                    UserTobeChanged.City = appUser.City;
                    UserTobeChanged.State = appUser.State;
                    UserTobeChanged.ZIP = appUser.ZIP;
                    UserTobeChanged.Email = appUser.Email;
                    UserTobeChanged.PhoneNumber = appUser.PhoneNumber;
                    db.Entry(UserTobeChanged).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("Employee") && appUser.Id != tempid)
                {
                    AppUser UserTobeChanged = db.Users.Find(appUser.Id);
                    UserTobeChanged.LName = appUser.LName;
                    UserTobeChanged.FName = appUser.FName;
                    UserTobeChanged.Initial = appUser.Initial;
                    UserTobeChanged.StreetAddress = appUser.StreetAddress;
                    UserTobeChanged.City = appUser.City;
                    UserTobeChanged.State = appUser.State;
                    UserTobeChanged.ZIP = appUser.ZIP;
                    UserTobeChanged.Email = appUser.Email;
                    UserTobeChanged.PhoneNumber = appUser.PhoneNumber;
                    db.Entry(UserTobeChanged).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
                return View(appUser);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.Users.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AppUser appUser = db.Users.Find(id);
            db.Users.Remove(appUser);
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

