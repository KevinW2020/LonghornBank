namespace LonghornBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Checkings",
                c => new
                    {
                        CheckingID = c.Int(nullable: false, identity: true),
                        CheckingNumber = c.Int(nullable: false),
                        CheckingName = c.String(nullable: false),
                        CheckingBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CheckingID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LName = c.String(),
                        FName = c.String(),
                        Initial = c.String(),
                        ActiveUser = c.Boolean(nullable: false),
                        DOB = c.DateTime(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.Int(nullable: false),
                        ZIP = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.IRAs",
                c => new
                    {
                        IRAID = c.Int(nullable: false, identity: true),
                        IRANumber = c.Int(nullable: false),
                        IRAName = c.String(nullable: false),
                        IRABalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Contribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IRAID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.Transfers",
                c => new
                    {
                        TransferID = c.Int(nullable: false, identity: true),
                        TransferDate = c.DateTime(nullable: false),
                        TransferAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransferDescription = c.String(nullable: false),
                        TransferComments = c.String(),
                        IRA_IRAID = c.Int(),
                        StockPortfolio_StockPortfolioID = c.Int(),
                    })
                .PrimaryKey(t => t.TransferID)
                .ForeignKey("dbo.IRAs", t => t.IRA_IRAID)
                .ForeignKey("dbo.StockPortfolios", t => t.StockPortfolio_StockPortfolioID)
                .Index(t => t.IRA_IRAID)
                .Index(t => t.StockPortfolio_StockPortfolioID);
            
            CreateTable(
                "dbo.Savings",
                c => new
                    {
                        SavingsID = c.Int(nullable: false, identity: true),
                        SavingsNumber = c.Int(nullable: false),
                        SavingsName = c.String(nullable: false),
                        SavingsBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SavingsID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.StockPortfolios",
                c => new
                    {
                        StockPortfolioID = c.Int(nullable: false, identity: true),
                        StockPortfolioNumber = c.Int(nullable: false),
                        StockPortfolioName = c.String(nullable: false),
                        CashBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceStatus = c.String(),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.StockPortfolioID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.StockBridges",
                c => new
                    {
                        StockBridgeID = c.Int(nullable: false, identity: true),
                        NumberShares = c.Int(nullable: false),
                        OriginalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseDate = c.DateTime(nullable: false),
                        StockList_StockListID = c.Int(),
                        StockPortfolio_StockPortfolioID = c.Int(),
                    })
                .PrimaryKey(t => t.StockBridgeID)
                .ForeignKey("dbo.StockLists", t => t.StockList_StockListID)
                .ForeignKey("dbo.StockPortfolios", t => t.StockPortfolio_StockPortfolioID)
                .Index(t => t.StockList_StockListID)
                .Index(t => t.StockPortfolio_StockPortfolioID);
            
            CreateTable(
                "dbo.StockLists",
                c => new
                    {
                        StockListID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Ticker = c.String(nullable: false),
                        StockType = c.Int(nullable: false),
                        TransactionFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StockListID);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Payees",
                c => new
                    {
                        PayeeID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StreetAddress = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.Int(nullable: false),
                        ZIP = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        PayeeType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PayeeID);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionID = c.Int(nullable: false, identity: true),
                        TypeOfTransaction = c.Int(nullable: false),
                        ApprovalStatus = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDescription = c.String(nullable: false),
                        TransactionComments = c.String(),
                        ManagerComments = c.String(),
                        ManagerEmail = c.String(),
                        AccountSender = c.Int(nullable: false),
                        AccountReceiver = c.Int(nullable: false),
                        AppUser_Id = c.String(maxLength: 128),
                        Payee_PayeeID = c.Int(),
                    })
                .PrimaryKey(t => t.TransactionID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .ForeignKey("dbo.Payees", t => t.Payee_PayeeID)
                .Index(t => t.AppUser_Id)
                .Index(t => t.Payee_PayeeID);
            
            CreateTable(
                "dbo.Disputes",
                c => new
                    {
                        DisputeID = c.Int(nullable: false),
                        DisputeStatus = c.Int(nullable: false),
                        Comments = c.String(nullable: false),
                        ManagerComments = c.String(),
                        CorrectAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeleteTransaction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DisputeID)
                .ForeignKey("dbo.Transactions", t => t.DisputeID)
                .Index(t => t.DisputeID);
            
            CreateTable(
                "dbo.Pendings",
                c => new
                    {
                        PendingID = c.Int(nullable: false, identity: true),
                        PendingStatus = c.Int(nullable: false),
                        AccountType = c.String(),
                        AccountNumber = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PendingID)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.TransferCheckings",
                c => new
                    {
                        Transfer_TransferID = c.Int(nullable: false),
                        Checking_CheckingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Transfer_TransferID, t.Checking_CheckingID })
                .ForeignKey("dbo.Transfers", t => t.Transfer_TransferID, cascadeDelete: true)
                .ForeignKey("dbo.Checkings", t => t.Checking_CheckingID, cascadeDelete: true)
                .Index(t => t.Transfer_TransferID)
                .Index(t => t.Checking_CheckingID);
            
            CreateTable(
                "dbo.SavingsTransfers",
                c => new
                    {
                        Savings_SavingsID = c.Int(nullable: false),
                        Transfer_TransferID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Savings_SavingsID, t.Transfer_TransferID })
                .ForeignKey("dbo.Savings", t => t.Savings_SavingsID, cascadeDelete: true)
                .ForeignKey("dbo.Transfers", t => t.Transfer_TransferID, cascadeDelete: true)
                .Index(t => t.Savings_SavingsID)
                .Index(t => t.Transfer_TransferID);
            
            CreateTable(
                "dbo.PayeeAppUsers",
                c => new
                    {
                        Payee_PayeeID = c.Int(nullable: false),
                        AppUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Payee_PayeeID, t.AppUser_Id })
                .ForeignKey("dbo.Payees", t => t.Payee_PayeeID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id, cascadeDelete: true)
                .Index(t => t.Payee_PayeeID)
                .Index(t => t.AppUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Pendings", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transactions", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.Disputes", "DisputeID", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PayeeAppUsers", "Payee_PayeeID", "dbo.Payees");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transfers", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.StockBridges", "StockPortfolio_StockPortfolioID", "dbo.StockPortfolios");
            DropForeignKey("dbo.StockBridges", "StockList_StockListID", "dbo.StockLists");
            DropForeignKey("dbo.StockPortfolios", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SavingsTransfers", "Transfer_TransferID", "dbo.Transfers");
            DropForeignKey("dbo.SavingsTransfers", "Savings_SavingsID", "dbo.Savings");
            DropForeignKey("dbo.Savings", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transfers", "IRA_IRAID", "dbo.IRAs");
            DropForeignKey("dbo.TransferCheckings", "Checking_CheckingID", "dbo.Checkings");
            DropForeignKey("dbo.TransferCheckings", "Transfer_TransferID", "dbo.Transfers");
            DropForeignKey("dbo.IRAs", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Checkings", "AppUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PayeeAppUsers", new[] { "AppUser_Id" });
            DropIndex("dbo.PayeeAppUsers", new[] { "Payee_PayeeID" });
            DropIndex("dbo.SavingsTransfers", new[] { "Transfer_TransferID" });
            DropIndex("dbo.SavingsTransfers", new[] { "Savings_SavingsID" });
            DropIndex("dbo.TransferCheckings", new[] { "Checking_CheckingID" });
            DropIndex("dbo.TransferCheckings", new[] { "Transfer_TransferID" });
            DropIndex("dbo.Pendings", new[] { "AppUser_Id" });
            DropIndex("dbo.Disputes", new[] { "DisputeID" });
            DropIndex("dbo.Transactions", new[] { "Payee_PayeeID" });
            DropIndex("dbo.Transactions", new[] { "AppUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.StockBridges", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.StockBridges", new[] { "StockList_StockListID" });
            DropIndex("dbo.StockPortfolios", new[] { "AppUser_Id" });
            DropIndex("dbo.Savings", new[] { "AppUser_Id" });
            DropIndex("dbo.Transfers", new[] { "StockPortfolio_StockPortfolioID" });
            DropIndex("dbo.Transfers", new[] { "IRA_IRAID" });
            DropIndex("dbo.IRAs", new[] { "AppUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Checkings", new[] { "AppUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.PayeeAppUsers");
            DropTable("dbo.SavingsTransfers");
            DropTable("dbo.TransferCheckings");
            DropTable("dbo.Pendings");
            DropTable("dbo.Disputes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Payees");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.StockLists");
            DropTable("dbo.StockBridges");
            DropTable("dbo.StockPortfolios");
            DropTable("dbo.Savings");
            DropTable("dbo.Transfers");
            DropTable("dbo.IRAs");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Checkings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
