namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddBedKingToRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rooms", "BedKind", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rooms", "BedKind");
        }
    }
}
