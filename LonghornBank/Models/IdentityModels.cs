using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using LonghornBank.Models;

namespace LonghornBank.Models
{
    public enum StateAbbr { AK, AL, AR, AZ, CA, CO, CT, DC, DE, FL, GA, HI, IA, ID, IL, IN, KS, KY, LA, MA, MD, ME, MI, MN, MO, MS, MT, NC, ND, NE, NH, NJ, NM, NV, NY, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VA, VT, WA, WI, WV, WY }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AppUser : IdentityUser
    {

        //TODO: Put any additional fields that you need for your user here
        //For instance
        [Display(Name = "Last Name")]
        public String LName { get; set; }

        [Display(Name = "First Name")]
        public String FName { get; set; }

        [Display(Name = "Middle Initial")]
        public String Initial { get; set; }
        public Boolean ActiveUser { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Display(Name = "Street Address")]
        public String StreetAddress { get; set; }
        public String City { get; set; }
        public StateAbbr State { get; set; }
        public String ZIP { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<Checking> Checkings { get; set; }
        public virtual List<Savings> Savings { get; set; }
        public virtual List<Pending> Pendings { get; set; }
        public virtual ICollection<IRA> IRAs { get; set; }
        public virtual ICollection<StockPortfolio> StockPortfolios { get; set; }
        public virtual ICollection<Payee> Payees { get; set; }


        //This method allows you to create a new user
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //TODO: Here's your db context for the project.  All of your db sets should go in here
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>().HasOptional(t => t.Disputes).WithRequired(m => m.Transaction);
        }
        //Remember, Identity adds a db set for users, so you shouldn't add that one - you will get an error

        public DbSet<Checking> Checkings { get; set; }
        public DbSet<Savings> Savings { get; set; }
        public DbSet<IRA> IRAs { get; set; }
        public DbSet<StockPortfolio> StockPortfolios { get; set; }
        public DbSet<StockList> StockLists { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Pending> Pendings { get; set; }


        //TODO: Make sure that your connection string name is correct here.
        public AppDbContext()
            : base("MyDBConnection", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public DbSet<AppRole> AppRoles { get; set; }

        public System.Data.Entity.DbSet<LonghornBank.Models.Dispute> Disputes { get; set; }
    }
}