namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPaymentToBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "TerminatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Bookings", "Payment_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Bookings", "Payment_Ammount", c => c.Double(nullable: false));
            AddColumn("dbo.Bookings", "Payment_Mode", c => c.Int(nullable: false));
            DropColumn("dbo.Bookings", "State");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "State", c => c.Int(nullable: false));
            DropColumn("dbo.Bookings", "Payment_Mode");
            DropColumn("dbo.Bookings", "Payment_Ammount");
            DropColumn("dbo.Bookings", "Payment_Date");
            DropColumn("dbo.Bookings", "TerminatedDate");
        }
    }
}
