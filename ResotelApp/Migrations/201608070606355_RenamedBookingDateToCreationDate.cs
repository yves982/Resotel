namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RenamedBookingDateToCreationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "CreationDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Bookings", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "Date", c => c.DateTime(nullable: false));
            DropColumn("dbo.Bookings", "CreationDate");
        }
    }
}
