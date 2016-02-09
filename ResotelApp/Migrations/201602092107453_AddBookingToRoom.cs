namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookingToRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rooms", "Booking_BookingId", c => c.Int());
            CreateIndex("dbo.Rooms", "Booking_BookingId");
            AddForeignKey("dbo.Rooms", "Booking_BookingId", "dbo.Bookings", "BookingId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "Booking_BookingId", "dbo.Bookings");
            DropIndex("dbo.Rooms", new[] { "Booking_BookingId" });
            DropColumn("dbo.Rooms", "Booking_BookingId");
        }
    }
}
