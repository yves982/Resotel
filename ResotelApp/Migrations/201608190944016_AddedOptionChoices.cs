namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOptionChoices : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Options", "Booking_Id", "dbo.Bookings");
            DropIndex("dbo.Options", new[] { "Booking_Id" });
            CreateTable(
                "dbo.OptionChoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Option_Id = c.Int(nullable: false),
                        TakenDates_Id = c.Int(nullable: false),
                        Booking_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Options", t => t.Option_Id, cascadeDelete: true)
                .ForeignKey("dbo.DateRanges", t => t.TakenDates_Id, cascadeDelete: true)
                .ForeignKey("dbo.Bookings", t => t.Booking_Id)
                .Index(t => t.Option_Id)
                .Index(t => t.TakenDates_Id)
                .Index(t => t.Booking_Id);
            
            DropColumn("dbo.Options", "Booking_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Options", "Booking_Id", c => c.Int());
            DropForeignKey("dbo.OptionChoices", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.OptionChoices", "TakenDates_Id", "dbo.DateRanges");
            DropForeignKey("dbo.OptionChoices", "Option_Id", "dbo.Options");
            DropIndex("dbo.OptionChoices", new[] { "Booking_Id" });
            DropIndex("dbo.OptionChoices", new[] { "TakenDates_Id" });
            DropIndex("dbo.OptionChoices", new[] { "Option_Id" });
            DropTable("dbo.OptionChoices");
            CreateIndex("dbo.Options", "Booking_Id");
            AddForeignKey("dbo.Options", "Booking_Id", "dbo.Bookings", "Id");
        }
    }
}
