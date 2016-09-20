namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MadeTerminatedDateOptionnal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "TerminatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "TerminatedDate", c => c.DateTime(nullable: false));
        }
    }
}
