namespace ResotelApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients");
            DropIndex("dbo.Bookings", new[] { "Client_ClientId" });
            AlterColumn("dbo.Bookings", "Client_ClientId", c => c.Int(nullable: false));
            AlterColumn("dbo.Clients", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Clients", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Options", "Label", c => c.String(nullable: false));
            CreateIndex("dbo.Bookings", "Client_ClientId");
            AddForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients", "ClientId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients");
            DropIndex("dbo.Bookings", new[] { "Client_ClientId" });
            AlterColumn("dbo.Options", "Label", c => c.String());
            AlterColumn("dbo.Clients", "LastName", c => c.String());
            AlterColumn("dbo.Clients", "FirstName", c => c.String());
            AlterColumn("dbo.Bookings", "Client_ClientId", c => c.Int());
            CreateIndex("dbo.Bookings", "Client_ClientId");
            AddForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients", "ClientId");
        }
    }
}
