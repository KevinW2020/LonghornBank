using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LonghornBank.Models;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Data;
namespace LonghornBank.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private AppUserManager _userManager;
        private AppDbContext db = new AppDbContext();

        public AccountController()
        {
        }

        public AccountController(AppUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated) //user has been redirected here from a page they're not authorized to see
            {
                return View("Error", new string[] { "Access Denied" });
            }
            AuthenticationManager.SignOut(); //this removes any old cookies hanging around
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.EmailAddress, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //// Redirect after login here

                    
                    //return RedirectToLocal(returnUrl);

                    //if (await UserManager.IsInRoleAsync(user.Id, "Customer")) //<= Checking Role and redirecting accordingly.
                    //    return RedirectToAction("CustomerHome", "Account");
                    //else
                    //    return RedirectToAction("Index", "User");




                    return RedirectToAction("RedirectUser");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult CustomerHome()
        {
            return View();
        }

        //
        public ActionResult RedirectUser()
        {
             //// Redirect after login here
                if (User.IsInRole("Customer"))
                {
                string user = User.Identity.GetUserId();
                AppUser u = db.Users.Find(user);

                var query1 = from p in u.Checkings
                             where p.AppUser.Id == user
                             select p.CheckingBalance;
                             

                query1 = query1.Where(q => q < 0);
                List<decimal> query1a = query1.ToList();

                var query2 = from p in u.IRAs
                             where p.AppUser.Id == user
                             select p.IRABalance;

                query2 = query2.Where(q => q < 0);
                List<decimal> query2a = query2.ToList();

                var query3 = from p in u.StockPortfolios
                             where p.AppUser.Id == user
                             select Convert.ToDecimal(p.CashBalance);

                query3 = query3.Where(q => q < 0);
                List<decimal> query3a = query3.ToList();
                var query4 = from p in u.Savings
                             where p.AppUser.Id == user

                             select p.SavingsBalance;


                query4 = query4.Where(q => q < 0);

                List<decimal> query4a = query4.ToList();

                if (u.IRAs.Count == 0 && u.StockPortfolios.Count == 0 && u.Savings.Count == 0 && u.Checkings.Count == 0)
                {
                    return RedirectToAction("Index", "Apply");
                }

                if (query1a.Count != 0 || query2a.Count != 0 || query3a.Count != 0 || query4a.Count != 0)
                {

                    Danger(string.Format("One or more of your account balances is overdrawn."), true);
                    return RedirectToAction("Index", "ManageAccounts");
                }

                else
                {
                    return RedirectToAction("Index", "ManageAccounts");
                }
                }
                else if (User.IsInRole("Manager"))
                {
                    return RedirectToAction("ManagerHome", "Account");
                }
                else if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("EmployeeHome", "Account");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            

        }


        [AllowAnonymous]
        [Authorize(Roles = "Manager")]
        public ActionResult ManagerHome()
        {
            return View();
        }

        [AllowAnonymous]
        [Authorize(Roles = "Manager, Employee")]
        public ActionResult EmployeeHome()
        {
            return View();
        }


        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Add fields to user here so they will be saved to do the database
                var user = new AppUser { UserName = model.EmailAddress, Email = model.EmailAddress, FName = model.FName, LName = model.LName, DOB = model.DOB, StreetAddress=model.Address, City = model.City, ZIP = model.ZIP, PhoneNumber=model.PhoneNumber, PasswordHash=model.Password, State = model.State
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                string user1 = User.Identity.GetUserId();
                AppUser u = db.Users.Find(user1);
                if (User.IsInRole("Manager"))
                {
                    await UserManager.AddToRoleAsync(user.Id, "Employee");
                    Success(string.Format("New Employee created!"), true);
                    return RedirectToAction("ManagerHome", "Account");

                }
                else
                {
                    await UserManager.AddToRoleAsync(user.Id, "Customer");
                }
                // --OR--
                // await UserManager.AddToRoleAsync(user.Id, "Employee");

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href="" + callbackUrl + "">here</a>");

                    return RedirectToAction("Index", "Apply");
                }


            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.EmailAddress);
                if (user == null)
                {

                    return View(model);
                }
                if (user != null)
                {
                    if (user.DOB == model.DOB)
                    {
                        EmailSender(user.Email);
                        return View("ForgotPasswordConfirmation");
                    }
                    else
                    {
                        return View("ForgotPasswordConfirmation");
                    }
                }
                else
                {
                    return View("ForgotPasswordConfirmation");
                }
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an EmailAddress with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAddressAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.EmailAddress);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            
            var token = UserManager.GeneratePasswordResetToken(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
            if (result.Succeeded)
            { 
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public static void EmailSender(String strEmail)
        {

            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "longhornbanking@gmail.com";
            string password = "longhornbanking";
            string emailTo = strEmail;
            string subject = "Team 5: Reset your Longhorn Bank Password";
            string body = "Reset your password by going to this link: localhost:57197/Account/ResetPassword";

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
        #region Helpers
            // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

          
           

        }
        #endregion
    }

}