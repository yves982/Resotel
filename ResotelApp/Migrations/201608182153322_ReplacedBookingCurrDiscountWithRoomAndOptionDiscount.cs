namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ReplacedBookingCurrDiscountWithRoomAndOptionDiscount : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts");
            DropIndex("dbo.Options", new[] { "CurrentDiscount_Id" });
            RenameColumn(table: "dbo.Bookings", name: "CurrentDiscount_Id", newName: "OptionDiscount_Id");
            RenameIndex(table: "dbo.Bookings", name: "IX_CurrentDiscount_Id", newName: "IX_OptionDiscount_Id");
            AddColumn("dbo.Bookings", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Discounts", "Option_Id", c => c.Int());
            AddColumn("dbo.Discounts", "Room_Id", c => c.Int());
            AddColumn("dbo.Rooms", "Price", c => c.Double(nullable: false));
            CreateIndex("dbo.Discounts", "Option_Id");
            CreateIndex("dbo.Discounts", "Room_Id");
            AddForeignKey("dbo.Discounts", "Option_Id", "dbo.Options", "Id");
            AddForeignKey("dbo.Discounts", "Room_Id", "dbo.Rooms", "Id");
            DropColumn("dbo.Options", "CurrentDiscount_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Options", "CurrentDiscount_Id", c => c.Int());
            DropForeignKey("dbo.Discounts", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.Discounts", "Option_Id", "dbo.Options");
            DropIndex("dbo.Discounts", new[] { "Room_Id" });
            DropIndex("dbo.Discounts", new[] { "Option_Id" });
            DropColumn("dbo.Rooms", "Price");
            DropColumn("dbo.Discounts", "Room_Id");
            DropColumn("dbo.Discounts", "Option_Id");
            DropColumn("dbo.Bookings", "State");
            RenameIndex(table: "dbo.Bookings", name: "IX_OptionDiscount_Id", newName: "IX_CurrentDiscount_Id");
            RenameColumn(table: "dbo.Bookings", name: "OptionDiscount_Id", newName: "CurrentDiscount_Id");
            CreateIndex("dbo.Options", "CurrentDiscount_Id");
            AddForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts", "Id");
        }
    }
}
