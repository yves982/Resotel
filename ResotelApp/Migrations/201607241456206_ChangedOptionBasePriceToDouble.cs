namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChangedOptionBasePriceToDouble : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Rights", c => c.Int(nullable: false));
            AlterColumn("dbo.Options", "BasePrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Options", "BasePrice", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "Rights");
        }
    }
}
