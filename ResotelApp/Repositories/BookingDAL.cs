using ResotelApp.Models;
using ResotelApp.Models.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ResotelApp.DAL
{
    public class BookingRepository
    {
        public async static Task<List<Booking>> GetAllBookings()
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                return await ctx.Bookings.ToListAsync();
            }
        }
    }
}
