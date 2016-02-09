namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Client_ClientId = c.Int(nullable: false),
                        CurrentDiscount_DiscountId = c.Int(),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Clients", t => t.Client_ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Discounts", t => t.CurrentDiscount_DiscountId)
                .Index(t => t.Client_ClientId)
                .Index(t => t.CurrentDiscount_DiscountId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        City = c.String(),
                        ZipCode = c.Int(nullable: false),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        DiscountId = c.Int(nullable: false, identity: true),
                        ReduceByPercent = c.Double(nullable: false),
                        ApplicableQuantity = c.Int(nullable: false),
                        Validity_DateRangeId = c.Int(),
                    })
                .PrimaryKey(t => t.DiscountId)
                .ForeignKey("dbo.DateRanges", t => t.Validity_DateRangeId)
                .Index(t => t.Validity_DateRangeId);
            
            CreateTable(
                "dbo.DateRanges",
                c => new
                    {
                        DateRangeId = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DateRangeId);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        OptionId = c.Int(nullable: false, identity: true),
                        Label = c.String(nullable: false),
                        BasePrice = c.Int(nullable: false),
                        Booking_BookingId = c.Int(),
                    })
                .PrimaryKey(t => t.OptionId)
                .ForeignKey("dbo.Bookings", t => t.Booking_BookingId)
                .Index(t => t.Booking_BookingId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        Stage = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoomId);
            
            CreateTable(
                "dbo.RoomOptions",
                c => new
                    {
                        Room_RoomId = c.Int(nullable: false),
                        Option_OptionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Room_RoomId, t.Option_OptionId })
                .ForeignKey("dbo.Rooms", t => t.Room_RoomId, cascadeDelete: true)
                .ForeignKey("dbo.Options", t => t.Option_OptionId, cascadeDelete: true)
                .Index(t => t.Room_RoomId)
                .Index(t => t.Option_OptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Options", "Booking_BookingId", "dbo.Bookings");
            DropForeignKey("dbo.RoomOptions", "Option_OptionId", "dbo.Options");
            DropForeignKey("dbo.RoomOptions", "Room_RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Bookings", "CurrentDiscount_DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.Discounts", "Validity_DateRangeId", "dbo.DateRanges");
            DropForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients");
            DropIndex("dbo.RoomOptions", new[] { "Option_OptionId" });
            DropIndex("dbo.RoomOptions", new[] { "Room_RoomId" });
            DropIndex("dbo.Options", new[] { "Booking_BookingId" });
            DropIndex("dbo.Discounts", new[] { "Validity_DateRangeId" });
            DropIndex("dbo.Bookings", new[] { "CurrentDiscount_DiscountId" });
            DropIndex("dbo.Bookings", new[] { "Client_ClientId" });
            DropTable("dbo.RoomOptions");
            DropTable("dbo.Rooms");
            DropTable("dbo.Options");
            DropTable("dbo.DateRanges");
            DropTable("dbo.Discounts");
            DropTable("dbo.Clients");
            DropTable("dbo.Bookings");
        }
    }
}
