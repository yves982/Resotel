using ResotelApp.Models;
using ResotelApp.Models.Context;
using ResotelApp.Utils;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ResotelApp.Repositories
{
    class RoomRepository
    {
        public static async Task<List<Room>> GetAvailablesBetweenAsync(DateRange dateRange)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                List<Room> availableRooms = await ctx.Rooms
                    .Include(room => room.Options.Select(opt => opt.Discounts.Select(discount => discount.Validity)))
                    .Include( room => room.AvailablePacks )
                    .Where(Room.NotBookedDuring(dateRange))
                    .ToListAsync();
                return availableRooms;
            }
        }

        public static async Task<List<Room>> GetMatchingRoomsBetween(IEnumerable<Option> options, DateRange dateRange)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                List<Room> matchingRooms = await ctx.Rooms
                    .Include( room => room.Options.Select( opt => opt.Discounts.Select( discount => discount.Validity ) ) )
                    .Include( room => room.AvailablePacks)
                    .Where(
                        Room.NotBookedDuring(dateRange)
                        .And(Room.WithOptions(options))
                    )
                    .ToListAsync();
                return matchingRooms;
            }
        }
    }
}