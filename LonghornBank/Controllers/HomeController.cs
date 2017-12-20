using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LonghornBank.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult ParentInfo()
        {

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ApplyForAccount()
        {
            return View();
        }

        public ActionResult MyHome()
        {
            if (User.IsInRole("Customer"))
            {
                return RedirectToAction("Index", "ManageAccounts");
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeHome", "Account");
            }
            else if (User.IsInRole("Manager"))
            {
                return RedirectToAction("ManagerHome", "Account");
            }
            else if(!User.IsInRole("Customer, Employee, Manager"))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}