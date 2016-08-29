namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDateRangeToBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "Dates_DateRangeId", c => c.Int());
            CreateIndex("dbo.Bookings", "Dates_DateRangeId");
            AddForeignKey("dbo.Bookings", "Dates_DateRangeId", "dbo.DateRanges", "DateRangeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "Dates_DateRangeId", "dbo.DateRanges");
            DropIndex("dbo.Bookings", new[] { "Dates_DateRangeId" });
            DropColumn("dbo.Bookings", "Dates_DateRangeId");
        }
    }
}
