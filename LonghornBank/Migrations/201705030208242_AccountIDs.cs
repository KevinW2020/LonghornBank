namespace LonghornBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountIDs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "AccountSenderID", c => c.Int(nullable: false));
            AddColumn("dbo.Transactions", "AccountReceiverID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "AccountReceiverID");
            DropColumn("dbo.Transactions", "AccountSenderID");
        }
    }
}
