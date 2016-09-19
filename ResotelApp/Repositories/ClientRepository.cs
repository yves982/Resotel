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
        public async static Task<Client> Save(Client client)
        {
            Client savedClient = null;
            using (ResotelContext ctx = new ResotelContext())
            {
                if (client.Id == 0)
                {
                    savedClient = await _saveNewClient(client, ctx);
                }
                else
                {
                    savedClient = await _editClient(client, ctx);
                }
            }
            return savedClient;
        }

        public async static Task<List<Client>> GetAllClients()
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                List<Client> clientEntities = await ctx.Clients
                    .Include(client => client.Bookings)
                    .Include(client => client.Bookings.Select(booking => booking.Dates))
                    .Include(client => client.Bookings.Select(booking => booking.RoomPacks))
                    .Include(client => client.Bookings.Select(booking => booking.RoomPacks.Select(appliedPack => appliedPack.Booking)))
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

        private static async Task<Client> _saveNewClient(Client client, ResotelContext ctx)
        {
            ctx.Clients.Add(client);
            await ctx.SaveChangesAsync();
            Client savedClient = client;
            return savedClient;
        }

        private static async Task<Client> _editClient(Client client, ResotelContext ctx)
        {
            Client editedClient = await ctx.Clients.FirstOrDefaultAsync(cl => cl.Id == client.Id);

            // a client only has bookings as reference type properties, and none of them will get changed while we edit the client.
            // so lets make sur entity knows this
            foreach(Booking booking in client.Bookings)
            {
                Booking trackedBooking = await ctx.Bookings
                    .Include(b => b.Dates)
                    .FirstOrDefaultAsync(b => b.Id == booking.Id);
                ctx.Entry(trackedBooking).State = EntityState.Unchanged;
            }

            // update all non reference properties
            ctx.Entry(editedClient).State = EntityState.Modified;
            ctx.Entry(editedClient).CurrentValues.SetValues(client);
            await ctx.SaveChangesAsync();
            return editedClient;
        }
    }
}
