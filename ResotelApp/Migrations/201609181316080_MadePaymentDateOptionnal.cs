namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadePaymentDateOptionnal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "Payment_Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "Payment_Date", c => c.DateTime(nullable: false));
        }
    }
}
