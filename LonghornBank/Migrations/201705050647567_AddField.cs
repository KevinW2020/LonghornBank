namespace LonghornBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Checkings", "Enableds", c => c.Int(nullable: false));
            AddColumn("dbo.IRAs", "Enableds", c => c.Int(nullable: false));
            AddColumn("dbo.Savings", "Enableds", c => c.Int(nullable: false));
            AddColumn("dbo.StockPortfolios", "Enableds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPortfolios", "Enableds");
            DropColumn("dbo.Savings", "Enableds");
            DropColumn("dbo.IRAs", "Enableds");
            DropColumn("dbo.Checkings", "Enableds");
        }
    }
}
