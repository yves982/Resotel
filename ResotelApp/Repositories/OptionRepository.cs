﻿using ResotelApp.Models;
using ResotelApp.Models.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ResotelApp.Repositories
{
    ///<summary>Repository to persist(CRUD operations) Options</summary>
    class OptionRepository
    {
        /// <summary>
        /// Gets Available Options within requested DateRange.
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        public static async Task<List<Option>> GetAvailablesBetweenAsync(DateRange dateRange)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                List<Option> availableOptions = await ctx.Options
                    .Include( opt => opt.Discounts.Select( discount => discount.Validity ) )
                    .Where(Option.IsAvailableBetween(dateRange))
                    .ToListAsync();
                return availableOptions;
            }
        }
    }
}
