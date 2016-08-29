using ResotelApp.Models;
using ResotelApp.Models.Context;
using System.Threading.Tasks;

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
    }
}
