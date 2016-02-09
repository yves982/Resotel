namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingRoomstoOption : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Options", "IsRoomRelated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Options", "IsRoomRelated", c => c.Boolean(nullable: false));
        }
    }
}
