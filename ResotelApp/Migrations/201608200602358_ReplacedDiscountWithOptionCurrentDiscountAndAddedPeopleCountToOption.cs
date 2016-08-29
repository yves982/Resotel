namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ReplacedDiscountWithOptionCurrentDiscountAndAddedPeopleCountToOption : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Discounts", "Option_Id", "dbo.OptionChoices");
            DropForeignKey("dbo.Bookings", "OptionDiscount_Id", "dbo.Discounts");
            DropForeignKey("dbo.Bookings", "CurrentDiscount_Id", "dbo.Discounts");
            DropForeignKey("dbo.Discounts", "Option_Id", "dbo.Options");
            DropIndex("dbo.Bookings", new[] { "OptionDiscount_Id" });
            DropIndex("dbo.Discounts", new[] { "Option_Id" });
            DropColumn("dbo.Bookings", "OptionDiscount_Id");
            DropColumn("dbo.Discounts", "Option_Id");
            AddColumn("dbo.OptionChoices", "PeopleCount", c => c.Int(nullable: false));
            AddColumn("dbo.Options", "CurrentDiscount_Id", c => c.Int());
            AddColumn("dbo.Discounts", "Booking_Id", c => c.Int());
            CreateIndex("dbo.Options", "CurrentDiscount_Id");
            CreateIndex("dbo.Discounts", "Booking_Id");
            AddForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts", "Id");
            AddForeignKey("dbo.Discounts", "Booking_Id", "dbo.Bookings", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Discounts", "Option_Id", c => c.Int());
            AddColumn("dbo.Bookings", "OptionDiscount_Id", c => c.Int());
            DropForeignKey("dbo.Discounts", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts");
            DropIndex("dbo.Discounts", new[] { "Booking_Id" });
            DropIndex("dbo.Options", new[] { "CurrentDiscount_Id" });
            DropColumn("dbo.Discounts", "Booking_Id");
            DropColumn("dbo.Options", "CurrentDiscount_Id");
            DropColumn("dbo.OptionChoices", "PeopleCount");
            CreateIndex("dbo.Discounts", "Option_Id");
            CreateIndex("dbo.Bookings", "OptionDiscount_Id");
            AddForeignKey("dbo.Bookings", "OptionDiscount_Id", "dbo.Discounts", "Id");
            AddForeignKey("dbo.Discounts", "Option_Id", "dbo.OptionChoices", "Id");
        }
    }
}
