using System.Data.Entity;

namespace ResotelApp.Models.Context
{
    class ResotelContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ResotelContext()
            :base("Resotel")
        {

        }
    }
}
