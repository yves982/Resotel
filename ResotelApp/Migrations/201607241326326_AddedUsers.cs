namespace ResotelApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Rooms", "Booking_BookingId", "dbo.Bookings");
            DropForeignKey("dbo.Options", "Booking_BookingId", "dbo.Bookings");
            DropForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients");
            DropForeignKey("dbo.Bookings", "CurrentDiscount_DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.Options", "CurrentDiscount_DiscountId", "dbo.Discounts");
            DropForeignKey("dbo.Discounts", "Validity_DateRangeId", "dbo.DateRanges");
            DropForeignKey("dbo.Bookings", "Dates_DateRangeId", "dbo.DateRanges");
            DropForeignKey("dbo.RoomOptions", "Option_OptionId", "dbo.Options");
            DropForeignKey("dbo.RoomOptions", "Room_RoomId", "dbo.Rooms");
            RenameColumn(table: "dbo.Bookings", name: "Client_ClientId", newName: "Client_Id");
            RenameColumn(table: "dbo.Bookings", name: "CurrentDiscount_DiscountId", newName: "CurrentDiscount_Id");
            RenameColumn(table: "dbo.Bookings", name: "Dates_DateRangeId", newName: "Dates_Id");
            RenameColumn(table: "dbo.Options", name: "Booking_BookingId", newName: "Booking_Id");
            RenameColumn(table: "dbo.Rooms", name: "Booking_BookingId", newName: "Booking_Id");
            RenameColumn(table: "dbo.Discounts", name: "Validity_DateRangeId", newName: "Validity_Id");
            RenameColumn(table: "dbo.Options", name: "CurrentDiscount_DiscountId", newName: "CurrentDiscount_Id");
            RenameColumn(table: "dbo.RoomOptions", name: "Room_RoomId", newName: "Room_Id");
            RenameColumn(table: "dbo.RoomOptions", name: "Option_OptionId", newName: "Option_Id");
            RenameIndex(table: "dbo.Bookings", name: "IX_Client_ClientId", newName: "IX_Client_Id");
            RenameIndex(table: "dbo.Bookings", name: "IX_CurrentDiscount_DiscountId", newName: "IX_CurrentDiscount_Id");
            RenameIndex(table: "dbo.Bookings", name: "IX_Dates_DateRangeId", newName: "IX_Dates_Id");
            RenameIndex(table: "dbo.Discounts", name: "IX_Validity_DateRangeId", newName: "IX_Validity_Id");
            RenameIndex(table: "dbo.Options", name: "IX_CurrentDiscount_DiscountId", newName: "IX_CurrentDiscount_Id");
            RenameIndex(table: "dbo.Options", name: "IX_Booking_BookingId", newName: "IX_Booking_Id");
            RenameIndex(table: "dbo.Rooms", name: "IX_Booking_BookingId", newName: "IX_Booking_Id");
            RenameIndex(table: "dbo.RoomOptions", name: "IX_Room_RoomId", newName: "IX_Room_Id");
            RenameIndex(table: "dbo.RoomOptions", name: "IX_Option_OptionId", newName: "IX_Option_Id");
            DropPrimaryKey("dbo.Bookings");
            DropPrimaryKey("dbo.Clients");
            DropPrimaryKey("dbo.Discounts");
            DropPrimaryKey("dbo.DateRanges");
            DropPrimaryKey("dbo.Options");
            DropPrimaryKey("dbo.Rooms");
            DropColumn("dbo.Bookings", "BookingId");
            DropColumn("dbo.Clients", "ClientId");
            DropColumn("dbo.Discounts", "DiscountId");
            DropColumn("dbo.DateRanges", "DateRangeId");
            DropColumn("dbo.Options", "OptionId");
            DropColumn("dbo.Rooms", "RoomId");
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Password = c.String(nullable: false),
                        Login = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(),
                        Service = c.String(),
                        Manager = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Bookings", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Clients", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Discounts", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.DateRanges", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Options", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Rooms", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Bookings", "Id");
            AddPrimaryKey("dbo.Clients", "Id");
            AddPrimaryKey("dbo.Discounts", "Id");
            AddPrimaryKey("dbo.DateRanges", "Id");
            AddPrimaryKey("dbo.Options", "Id");
            AddPrimaryKey("dbo.Rooms", "Id");
            AddForeignKey("dbo.Rooms", "Booking_Id", "dbo.Bookings", "Id");
            AddForeignKey("dbo.Options", "Booking_Id", "dbo.Bookings", "Id");
            AddForeignKey("dbo.Bookings", "Client_Id", "dbo.Clients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Bookings", "CurrentDiscount_Id", "dbo.Discounts", "Id");
            AddForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts", "Id");
            AddForeignKey("dbo.Discounts", "Validity_Id", "dbo.DateRanges", "Id");
            AddForeignKey("dbo.Bookings", "Dates_Id", "dbo.DateRanges", "Id");
            AddForeignKey("dbo.RoomOptions", "Option_Id", "dbo.Options", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RoomOptions", "Room_Id", "dbo.Rooms", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rooms", "RoomId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Options", "OptionId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.DateRanges", "DateRangeId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Discounts", "DiscountId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Clients", "ClientId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Bookings", "BookingId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.RoomOptions", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.RoomOptions", "Option_Id", "dbo.Options");
            DropForeignKey("dbo.Bookings", "Dates_Id", "dbo.DateRanges");
            DropForeignKey("dbo.Discounts", "Validity_Id", "dbo.DateRanges");
            DropForeignKey("dbo.Options", "CurrentDiscount_Id", "dbo.Discounts");
            DropForeignKey("dbo.Bookings", "CurrentDiscount_Id", "dbo.Discounts");
            DropForeignKey("dbo.Bookings", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Options", "Booking_Id", "dbo.Bookings");
            DropForeignKey("dbo.Rooms", "Booking_Id", "dbo.Bookings");
            DropPrimaryKey("dbo.Rooms");
            DropPrimaryKey("dbo.Options");
            DropPrimaryKey("dbo.DateRanges");
            DropPrimaryKey("dbo.Discounts");
            DropPrimaryKey("dbo.Clients");
            DropPrimaryKey("dbo.Bookings");
            DropColumn("dbo.Rooms", "Id");
            DropColumn("dbo.Options", "Id");
            DropColumn("dbo.DateRanges", "Id");
            DropColumn("dbo.Discounts", "Id");
            DropColumn("dbo.Clients", "Id");
            DropColumn("dbo.Bookings", "Id");
            DropTable("dbo.Users");
            AddPrimaryKey("dbo.Rooms", "RoomId");
            AddPrimaryKey("dbo.Options", "OptionId");
            AddPrimaryKey("dbo.DateRanges", "DateRangeId");
            AddPrimaryKey("dbo.Discounts", "DiscountId");
            AddPrimaryKey("dbo.Clients", "ClientId");
            AddPrimaryKey("dbo.Bookings", "BookingId");
            RenameIndex(table: "dbo.RoomOptions", name: "IX_Option_Id", newName: "IX_Option_OptionId");
            RenameIndex(table: "dbo.RoomOptions", name: "IX_Room_Id", newName: "IX_Room_RoomId");
            RenameIndex(table: "dbo.Rooms", name: "IX_Booking_Id", newName: "IX_Booking_BookingId");
            RenameIndex(table: "dbo.Options", name: "IX_Booking_Id", newName: "IX_Booking_BookingId");
            RenameIndex(table: "dbo.Options", name: "IX_CurrentDiscount_Id", newName: "IX_CurrentDiscount_DiscountId");
            RenameIndex(table: "dbo.Discounts", name: "IX_Validity_Id", newName: "IX_Validity_DateRangeId");
            RenameIndex(table: "dbo.Bookings", name: "IX_Dates_Id", newName: "IX_Dates_DateRangeId");
            RenameIndex(table: "dbo.Bookings", name: "IX_CurrentDiscount_Id", newName: "IX_CurrentDiscount_DiscountId");
            RenameIndex(table: "dbo.Bookings", name: "IX_Client_Id", newName: "IX_Client_ClientId");
            RenameColumn(table: "dbo.RoomOptions", name: "Option_Id", newName: "Option_OptionId");
            RenameColumn(table: "dbo.RoomOptions", name: "Room_Id", newName: "Room_RoomId");
            RenameColumn(table: "dbo.Options", name: "CurrentDiscount_Id", newName: "CurrentDiscount_DiscountId");
            RenameColumn(table: "dbo.Discounts", name: "Validity_Id", newName: "Validity_DateRangeId");
            RenameColumn(table: "dbo.Rooms", name: "Booking_Id", newName: "Booking_BookingId");
            RenameColumn(table: "dbo.Options", name: "Booking_Id", newName: "Booking_BookingId");
            RenameColumn(table: "dbo.Bookings", name: "Dates_Id", newName: "Dates_DateRangeId");
            RenameColumn(table: "dbo.Bookings", name: "CurrentDiscount_Id", newName: "CurrentDiscount_DiscountId");
            RenameColumn(table: "dbo.Bookings", name: "Client_Id", newName: "Client_ClientId");
            AddForeignKey("dbo.RoomOptions", "Room_RoomId", "dbo.Rooms", "RoomId", cascadeDelete: true);
            AddForeignKey("dbo.RoomOptions", "Option_OptionId", "dbo.Options", "OptionId", cascadeDelete: true);
            AddForeignKey("dbo.Bookings", "Dates_DateRangeId", "dbo.DateRanges", "DateRangeId");
            AddForeignKey("dbo.Discounts", "Validity_DateRangeId", "dbo.DateRanges", "DateRangeId");
            AddForeignKey("dbo.Options", "CurrentDiscount_DiscountId", "dbo.Discounts", "DiscountId");
            AddForeignKey("dbo.Bookings", "CurrentDiscount_DiscountId", "dbo.Discounts", "DiscountId");
            AddForeignKey("dbo.Bookings", "Client_ClientId", "dbo.Clients", "ClientId", cascadeDelete: true);
            AddForeignKey("dbo.Options", "Booking_BookingId", "dbo.Bookings", "BookingId");
            AddForeignKey("dbo.Rooms", "Booking_BookingId", "dbo.Bookings", "BookingId");
        }
    }
}
