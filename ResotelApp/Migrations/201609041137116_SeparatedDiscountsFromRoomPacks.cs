namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeparatedDiscountsFromRoomPacks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts");
            DropForeignKey("dbo.AppliedDiscounts", "Discount_Id", "dbo.Discounts");
            DropForeignKey("dbo.AppliedDiscounts", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.AppliedDiscounts", "Booking_Id", "dbo.Bookings");
            DropIndex("dbo.Options", new[] { "CurrentDiscount_Id" });
            DropIndex("dbo.Discounts", new[] { "Room_Id" });
            DropIndex("dbo.AppliedDiscounts", new[] { "Discount_Id" });
            DropIndex("dbo.AppliedDiscounts", new[] { "Room_Id" });
            DropIndex("dbo.AppliedDiscounts", new[] { "Booking_Id" });
            CreateTable(
                "dbo.Packs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.AppliedPacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                        Room_Id = c.Int(),
                        RoomPack_Id = c.Int(),
                        Booking_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.Room_Id)
                .ForeignKey("dbo.Packs", t => t.RoomPack_Id)
                .ForeignKey("dbo.Bookings", t => t.Booking_Id)
                .Index(t => t.Room_Id)
                .Index(t => t.RoomPack_Id)
                .Index(t => t.Booking_Id);
            
            AddColumn("dbo.Discounts", "Option_Id", c => c.Int());
            CreateIndex("dbo.Discounts", "Option_Id");
            AddForeignKey("dbo.Discounts", "Option_Id", "dbo.Options", "Id");
            DropColumn("dbo.Options", "CurrentDiscount_Id");
            DropColumn("dbo.Discounts", "PackPrice");
            DropColumn("dbo.Discounts", "PackQuantity");
            DropForeignKey("dbo.Discounts", "Room_Id", "dbo.Rooms");
            DropColumn("dbo.Discounts", "Room_Id");
            DropTable("dbo.AppliedDiscounts");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Discounts", "Room_Id", c => c.Int());
            AddColumn("dbo.Discounts", "PackQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Discounts", "PackPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Options", "CurrentDiscount_Id", c => c.Int());
            DropForeignKey("dbo.AppliedPacks", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.AppliedPacks", "RoomPack_Id", "dbo.Packs");
            DropForeignKey("dbo.AppliedPacks", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.Discounts", "Option_Id", "dbo.Options");
            DropIndex("dbo.AppliedPacks", new[] { "Booking_Id" });
            DropIndex("dbo.AppliedPacks", new[] { "RoomPack_Id" });
            DropIndex("dbo.AppliedPacks", new[] { "Room_Id" });
            DropIndex("dbo.Packs", new[] { "Room_Id" });
            DropIndex("dbo.Discounts", new[] { "Option_Id" });
            DropColumn("dbo.Discounts", "Option_Id");
            DropTable("dbo.AppliedPacks");
            DropTable("dbo.Packs");
            CreateIndex("dbo.AppliedDiscounts", "Booking_Id");
            CreateIndex("dbo.AppliedDiscounts", "Room_Id");
            CreateIndex("dbo.AppliedDiscounts", "Discount_Id");
            CreateIndex("dbo.Discounts", "Room_Id");
            CreateIndex("dbo.Options", "CurrentDiscount_Id");
            AddForeignKey("dbo.AppliedDiscounts", "Booking_Id", "dbo.Bookings", "Id");
            AddForeignKey("dbo.AppliedDiscounts", "Room_Id", "dbo.Rooms", "Id");
            AddForeignKey("dbo.AppliedDiscounts", "Discount_Id", "dbo.Discounts", "Id");
            AddForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts", "Id");
        }
    }
}
