namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedManyBookingsToRoom : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Rooms", "Booking_Id", "dbo.Bookings");
            DropIndex("dbo.Rooms", new[] { "Booking_Id" });
            CreateTable(
                "dbo.RoomBookings",
                c => new
                    {
                        Room_Id = c.Int(nullable: false),
                        Booking_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Room_Id, t.Booking_Id })
                .ForeignKey("dbo.Rooms", t => t.Room_Id, cascadeDelete: true)
                .ForeignKey("dbo.Bookings", t => t.Booking_Id, cascadeDelete: true)
                .Index(t => t.Room_Id)
                .Index(t => t.Booking_Id);
            
            DropColumn("dbo.Rooms", "Booking_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rooms", "Booking_Id", c => c.Int());
            DropForeignKey("dbo.RoomBookings", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.RoomBookings", "Room_Id", "dbo.Rooms");
            DropIndex("dbo.RoomBookings", new[] { "Booking_Id" });
            DropIndex("dbo.RoomBookings", new[] { "Room_Id" });
            DropTable("dbo.RoomBookings");
            CreateIndex("dbo.Rooms", "Booking_Id");
            AddForeignKey("dbo.Rooms", "Booking_Id", "dbo.Bookings", "Id");
        }
    }
}
