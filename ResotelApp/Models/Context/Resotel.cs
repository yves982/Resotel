using System.Data.Entity;

namespace ResotelApp.Models.Context
{
    class Resotel : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
