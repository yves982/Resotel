using ResotelApp.Models;
using ResotelApp.Models.Context;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ResotelApp.Repositories
{
    class UserRepository
    {
        public static async Task<User> FindByLoginAsync(string login)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                User foundUser = await ctx.Users.FirstOrDefaultAsync(u => 
                    u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase)
                );
                return foundUser;
            }
        }
    }
}