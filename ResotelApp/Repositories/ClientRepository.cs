using ResotelApp.Models;
using ResotelApp.Models.Context;
using System.Threading.Tasks;
using ResotelApp.ViewModels.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ResotelApp.Repositories
{
    class ClientRepository
    {
        public async static Task<Client> SaveNewClient(Client client)
        {
            Client newClient = null;
            using (ResotelContext ctx = new ResotelContext())
            {
                ctx.Clients.Add(client);
                await ctx.SaveChangesAsync();
                newClient = client;
            }
            return newClient;
        }

        public async static Task<List<Client>> GetAllClients()
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                List<Client> clientEntities = await ctx.Clients
                    .Include(client => client.Bookings)
                    .Include(client => client.Bookings.Select(booking => booking.Dates))
                    .Include(client => client.Bookings.Select(booking => booking.RoomPacks))
                    .Include(client => client.Bookings.Select(booking => booking.RoomPacks.Select(appliedPack => appliedPack.Room)))
                    .Include(client => client.Bookings.Select(booking => booking.RoomPacks.Select(appliedPack => appliedPack.RoomPack)))
                    .Include(client => client.Bookings.Select(booking => booking.Rooms))
                    .Include(client => client.Bookings.Select(booking => booking.Rooms.Select( r=> r.AvailablePacks )))
                    .Include(client => client.Bookings.Select(booking => booking.OptionChoices))
                    .Include(client => client.Bookings.Select(booking => booking.OptionChoices.Select(optC => optC.TakenDates)))
                    .Include(client => client.Bookings.Select(booking => booking.OptionChoices.Select(optC => optC.Option)))
                    .Include(client => client.Bookings.Select(booking => booking.OptionChoices.Select(optC => optC.Option.Discounts)))
                    .ToListAsync();
                return clientEntities;
            }
        }
    }
}
