using System.Data.Entity;

namespace ResotelApp.Models.Context
{
    class ResotelContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Option> Options { get; set; }

        public ResotelContext()
            :base("Resotel")
        {

        }
    }
}
