namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPersonsCountToBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "AdultsCount", c => c.Int(nullable: false));
            AddColumn("dbo.Bookings", "BabiesCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "BabiesCount");
            DropColumn("dbo.Bookings", "AdultsCount");
        }
    }
}
