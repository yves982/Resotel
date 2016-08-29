namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RenamedRoomSizeToCapacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rooms", "Capacity", c => c.Int(nullable: false));
            DropColumn("dbo.Rooms", "Size");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rooms", "Size", c => c.Int(nullable: false));
            DropColumn("dbo.Rooms", "Capacity");
        }
    }
}
