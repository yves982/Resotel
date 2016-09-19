namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHasChooseableDatesToOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Options", "HasChooseableDates", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Options", "HasChooseableDates");
        }
    }
}
