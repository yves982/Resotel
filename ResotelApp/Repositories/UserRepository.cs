using ResotelApp.Models;
using ResotelApp.Models.Context;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ResotelApp.Repositories
{
    ///<summary>Repository to persist(CRUD operations) Users</summary>
    class UserRepository
    {
        /// <summary>
        /// Gets a User by its login (as this one should be unique)
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
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