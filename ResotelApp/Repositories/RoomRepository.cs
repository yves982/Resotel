using ResotelApp.Models;
using ResotelApp.Models.Context;
using ResotelApp.Utils;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ResotelApp.Repositories
{
    ///<summary>Repository to persist(CRUD operations) Rooms</summary>
    class RoomRepository
    {
        /// <summary>
        /// Gets all available Rooms wihin requested dateRange
        /// A Room is available for a given dateRange if it has no booking including those dates save the last which may be on a booking's startDate
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all Room with given Options available within requested dateRange
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dateRange"></param>
        /// <returns></returns>
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