namespace LonghornBank.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LonghornBank.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LonghornBank.Models.AppDbContext db)
        {
            AddCustomers.SeedCustomers(db);
            AddCustomers.SeedEmployees(db);
            AddCustomers.SeedAccounts(db);
            //AddCustomers.SeedTransactions(db);
            AddCustomers.SeedStocks(db);
            AddCustomers.SeedPayees(db);
        }
    }
}
