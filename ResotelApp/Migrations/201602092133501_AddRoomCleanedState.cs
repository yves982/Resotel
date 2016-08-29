namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddRoomCleanedState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rooms", "IsCleaned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rooms", "IsCleaned");
        }
    }
}
