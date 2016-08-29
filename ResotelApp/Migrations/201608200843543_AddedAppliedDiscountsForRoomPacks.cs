namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedAppliedDiscountsForRoomPacks : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Discounts", new[] { "Booking_Id" });
            CreateTable(
                "dbo.AppliedDiscounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                        Discount_Id = c.Int(),
                        Room_Id = c.Int(),
                        Booking_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discounts", t => t.Discount_Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .Index(t => t.Discount_Id)
                .Index(t => t.Room_Id)
                .Index(t => t.Booking_Id);
            
            AddColumn("dbo.Clients", "BirthDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Discounts", "PackPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Discounts", "PackQuantity", c => c.Int(nullable: false));
            DropForeignKey("dbo.Discounts", "Booking_Id", "dbo.Bookings");
            DropColumn("dbo.Discounts", "ApplicableQuantity");
            DropColumn("dbo.Discounts", "Booking_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Discounts", "Booking_Id", c => c.Int());
            AddColumn("dbo.Discounts", "ApplicableQuantity", c => c.Int(nullable: false));
            DropForeignKey("dbo.AppliedDiscounts", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.AppliedDiscounts", "Discount_Id", "dbo.Discounts");
            DropIndex("dbo.AppliedDiscounts", new[] { "Booking_Id" });
            DropIndex("dbo.AppliedDiscounts", new[] { "Room_Id" });
            DropIndex("dbo.AppliedDiscounts", new[] { "Discount_Id" });
            DropColumn("dbo.Discounts", "PackQuantity");
            DropColumn("dbo.Discounts", "PackPrice");
            DropColumn("dbo.Clients", "BirthDate");
            DropTable("dbo.AppliedDiscounts");
            CreateIndex("dbo.Discounts", "Booking_Id");
        }
    }
}
