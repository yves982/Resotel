namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddCurrentDiscountToOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Options", "CurrentDiscount_DiscountId", c => c.Int());
            CreateIndex("dbo.Options", "CurrentDiscount_DiscountId");
            AddForeignKey("dbo.Options", "CurrentDiscount_DiscountId", "dbo.Discounts", "DiscountId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Options", "CurrentDiscount_DiscountId", "dbo.Discounts");
            DropIndex("dbo.Options", new[] { "CurrentDiscount_DiscountId" });
            DropColumn("dbo.Options", "CurrentDiscount_DiscountId");
        }
    }
}
