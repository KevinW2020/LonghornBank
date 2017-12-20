namespace LonghornBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CanBuyPortfolio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockPortfolios", "CanBuy", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPortfolios", "CanBuy");
        }
    }
}
