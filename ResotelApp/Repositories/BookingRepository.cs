using ResotelApp.Models;
using ResotelApp.Models.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ResotelApp.DAL
{
    /// <summary>Repository to persist (CRUD operations) Bookings</summary>
    class BookingRepository
    {
        /// <summary>
        /// SaveOrUpdate given Booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>The saved booking, with its new id, if a new booking (which was never saved) is passed</returns>
        public async static Task<Booking> Save(Booking booking)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                Booking savedBooking = null;

                List<Room> rooms = await _getRoomRequest(ctx, true)
                        .ToListAsync();
                List<Option> options = await _getOptionRequest(ctx)
                    .ToListAsync();

                Client client = await _getClientRequest(ctx).FirstOrDefaultAsync(
                    cl => cl.Id == booking.Client.Id
                );

                _assignCtxRooms(booking, rooms);
                _assignCtxOptions(booking, options);

                if (client != null)
                {
                    booking.Client = client;
                }

                if (booking.Id == 0)
                {

                    ctx.Entry(booking).State = EntityState.Added;
                }
                else
                {
                    Booking trackedBooking = await ctx.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
                    
                    _updateTrackedBooking(trackedBooking, booking, ctx);
                    await _updateOptionChoices(trackedBooking.OptionChoices, booking.OptionChoices, ctx);
                    await _updateBookingRooms(trackedBooking, booking.Rooms, ctx);
                    await _updateRoomPacks(trackedBooking, trackedBooking.RoomPacks, booking.RoomPacks, ctx);
                    _updateTrackedBookingState(trackedBooking, booking.State, ctx);

                    DateRange trackedBookingDates = await _getTrackedBookingDates(booking.Dates, ctx);
                    trackedBooking.Dates = trackedBookingDates;
                }

                

                List<Room> unavailableRooms = new List<Room>();

                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        await _fillUnavailableRooms(booking, ctx, unavailableRooms);

                        if (unavailableRooms.Count == 0)
                        {
                            savedBooking = await _saveBooking(booking, ctx, transaction);
                        }
                        else
                        {
                            List<Room> unavailableReplacementRooms = await _replaceRooms(booking, ctx, unavailableRooms);

                            if (unavailableReplacementRooms.Count > 0)
                            {
                                HashSet<RoomKind> unreplaceableRoomKinds = new HashSet<RoomKind>(unavailableReplacementRooms.ConvertAll(room => room.Kind));
                                transaction.Rollback();
                                throw new InvalidOperationException(
                                    $"Impossible de sauvegarder la réservation : nous n'avons plus assez de chambres des types {string.Join(",", unreplaceableRoomKinds)}");
                            }
                            else
                            {
                                savedBooking = await _saveBooking(booking, ctx, transaction);
                            }
                        }

                        return savedBooking;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static async Task<DateRange> _getTrackedBookingDates(DateRange newBookingDates, ResotelContext ctx)
        {
            DateRange trackedBookingDates = await ctx.Set<DateRange>().FirstOrDefaultAsync(dr => dr.Id == newBookingDates.Id);
            if (trackedBookingDates == null)
            {
                trackedBookingDates = ctx.Set<DateRange>().Attach(newBookingDates);
            } else
            {
                // check
                ctx.Entry(trackedBookingDates).CurrentValues.SetValues(newBookingDates);
            }
            return trackedBookingDates;
        }

        private static void _updateTrackedBookingState(Booking trackedBooking, BookingState newBookingState, ResotelContext ctx)
        {
            ctx.Entry(trackedBooking).Property( b => b.State).EntityEntry.State = EntityState.Modified;
            ctx.Entry(trackedBooking).Property(b => b.State).EntityEntry.CurrentValues.SetValues(newBookingState);
        }

        private static async Task _updateRoomPacks(Booking trackedBooking, IList<AppliedPack> trackedRoomPacks, IList<AppliedPack> newRoomPacks, ResotelContext ctx)
        {
            trackedRoomPacks.Clear();
            foreach (AppliedPack appliedPack in newRoomPacks)
            {
                AppliedPack trackedAppliedPack = await ctx.Set<AppliedPack>().FirstOrDefaultAsync(ap => ap.Id == appliedPack.Id);
                if (trackedAppliedPack == null)
                {
                    trackedAppliedPack = ctx.Set<AppliedPack>().Add(appliedPack);
                }

                Room trackedRoom = await ctx.Rooms.FirstOrDefaultAsync(r => r.Id == appliedPack.Room.Id);
                Pack trackedPack = await ctx.Set<Pack>().FirstOrDefaultAsync(p => p.Id == appliedPack.RoomPack.Id);

                trackedAppliedPack.Room = trackedRoom;
                trackedAppliedPack.RoomPack = trackedPack;
                trackedAppliedPack.Booking = trackedBooking;
            }
        }

        private static async Task _updateBookingRooms(Booking trackedBooking, IList<Room> newRooms, ResotelContext ctx)
        {
            ctx.Entry(trackedBooking).Collection(b => b.Rooms).EntityEntry.State = EntityState.Modified;
            trackedBooking.Rooms.Clear();
            foreach (Room room in newRooms)
            {
                Room trackedRoom = await ctx.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);
                trackedBooking.Rooms.Add(trackedRoom);
            }
        }

        private static async Task _updateOptionChoices(IList<OptionChoice> trackedOptionChoices, IList<OptionChoice> newOptionsChoices, ResotelContext ctx)
        {
            // Deletion ain't no small thing in EF, that thing holds memory dear,
            // so once you delete something, all aggregated entities are set to null, fun, isn't it
            // db do have transactions to test and try things before committing, but EF does not ..
            // cancelling a deletion imply a new db request, how sweet
            List<OptionChoice> optionChoicesToRemove = trackedOptionChoices.
                Where(trackedOptC => !newOptionsChoices.Any(newOptC => newOptC.Id == trackedOptC.Id)).ToList();
            _removeExistingOptionChoices(optionChoicesToRemove, ctx);

            foreach (OptionChoice optChoice in newOptionsChoices)
            {
                OptionChoice trackedOptChoice = await _getTrackedOptionChoice(optChoice, ctx);
                await _getTrackedOption(optChoice, ctx);
                DateRange trackedOptDates = await _getTrackedOptionDates(optChoice, ctx);


                _updateTrackedOptionDates(trackedOptDates, optChoice.TakenDates, ctx);
                _updateTrackedOptionChoice(trackedOptChoice, optChoice, ctx);

                trackedOptionChoices.Add(trackedOptChoice);
            }
        }

        private static void _updateTrackedOptionChoice(OptionChoice trackedOptChoice, OptionChoice newOptChoice, ResotelContext ctx)
        {

            ctx.Entry(trackedOptChoice).CurrentValues.SetValues(newOptChoice);
        }

        private static void _updateTrackedOptionDates(DateRange trackedOptDates, DateRange newDateRange, ResotelContext ctx)
        {
            ctx.Entry(trackedOptDates).CurrentValues.SetValues(newDateRange);
        }

        private static void _updateTrackedBooking(Booking trackedBooking, Booking booking, ResotelContext ctx)
        {
            
            ctx.Entry(trackedBooking).State = EntityState.Modified;
            ctx.Entry(trackedBooking).CurrentValues.SetValues(booking);
        }

        private static void _removeExistingOptionChoices(IList<OptionChoice> trackedOptionChoices, ResotelContext ctx)
        {
            for (int i = trackedOptionChoices.Count - 1; i >= 0; i--)
            {
                OptionChoice trackedOptChoice = trackedOptionChoices[i];
                ctx.Entry(trackedOptChoice).State = EntityState.Deleted;
            }
        }

        private static async Task<OptionChoice> _getTrackedOptionChoice(OptionChoice optChoice, ResotelContext ctx)
        {
            OptionChoice trackedOptChoice = await ctx.Set<OptionChoice>()
                .Include(optC => optC.Option)
                .Include(optC => optC.Option.Discounts)
                .Include(optC => optC.Option.Rooms)
                .Include(optC => optC.TakenDates)
                .FirstOrDefaultAsync(optC => optC.Id == optChoice.Id);
            
            if (trackedOptChoice == null)
            {
                trackedOptChoice = ctx.Set<OptionChoice>().Add(optChoice);
            }

            return trackedOptChoice;
        }

        private static async Task _getTrackedOption(OptionChoice optChoice, ResotelContext ctx)
        {
            Option trackedOpt = await ctx.Options.FirstOrDefaultAsync(opt => opt.Id == optChoice.Id);
        }

        private static async Task<DateRange> _getTrackedOptionDates(OptionChoice optChoice, ResotelContext ctx)
        {
            DateRange trackedOptDates = await ctx.Set<DateRange>().FirstOrDefaultAsync(dr => dr.Id == optChoice.TakenDates.Id);
            if (trackedOptDates == null)
            {
                trackedOptDates = ctx.Set<DateRange>().Add(optChoice.TakenDates);
            }

            return trackedOptDates;
        }

        private static IQueryable<Client> _getClientRequest(ResotelContext ctx)
        {
            IQueryable<Client> clientRequest = ctx.Clients
                                .Include(cl => cl.Bookings)
                                .Include(cl => cl.Bookings.Select(b => b.Rooms))
                                .Include(cl => cl.Bookings.Select(b => b.Rooms.Select( r => r.Options )))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Booking)))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room)))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room.Options)))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.RoomPack)))
                                .Include(cl => cl.Bookings.Select(b => b.OptionChoices))
                                .Include(cl => cl.Bookings.Select(b => b.OptionChoices.Select(optC => optC.Option)))
                                .Include(cl => cl.Bookings.Select(b => b.OptionChoices.Select(optC => optC.Option.Rooms)))
                                .Include(cl => cl.Bookings.Select(b => b.OptionChoices.Select(optC => optC.TakenDates)));
            return clientRequest;
        }

        private static async Task<List<Room>> _replaceRooms(Booking booking, ResotelContext ctx, List<Room> unavailableRooms)
        {
            List<Room> unavailableReplacementRooms = new List<Room>();
            foreach (Room unavailableRoom in unavailableRooms)
            {
                Room replacementRoom = await _getReplacementRoom(ctx, unavailableRoom);

                if (replacementRoom != null)
                {
                    _replaceRoom(booking, unavailableRoom, replacementRoom);
                }
                else
                {
                    unavailableReplacementRooms.Add(unavailableRoom);
                }
            }
            return unavailableReplacementRooms;
        }

        private static void _replaceRoom(Booking booking, Room unavailableRoom, Room replacementRoom)
        {
            booking.Rooms.Add(replacementRoom);
            booking.Rooms.Remove(unavailableRoom);
        }

        private static async Task<Room> _getReplacementRoom(ResotelContext ctx, Room unavailableRoom)
        {
            return await _getRoomRequest(ctx, false)
                                                .FirstOrDefaultAsync(room => room.Kind == unavailableRoom.Kind);
        }

        private static async Task<Booking> _saveBooking(Booking booking, ResotelContext ctx, DbContextTransaction transaction)
        {
            await ctx.SaveChangesAsync();
            transaction.Commit();
            return booking;
        }

        private static async Task _fillUnavailableRooms(Booking booking, ResotelContext ctx, List<Room> unavailableRooms)
        {
            foreach (Room room in booking.Rooms)
            {
                Room freshRoom = await _getFreshRoom(ctx, room.Id);
                if (freshRoom == null)
                {
                    unavailableRooms.Add(freshRoom);
                }
            }
        }

        private async static Task<Room> _getFreshRoom(ResotelContext ctx, int roomId)
        {
            return await ctx.Rooms.AsNoTracking()
                .Where(r => r.Id == roomId)
                .FirstOrDefaultAsync();
        }

        private static void _assignCtxOptions(Booking booking, List<Option> options)
        {
            foreach (OptionChoice optChoice in booking.OptionChoices)
            {
                optChoice.Option = options.Find(opt => optChoice.Option.Id == opt.Id);
            }
        }

        private static void _assignCtxRooms(Booking booking, List<Room> rooms)
        {
            booking.Rooms = rooms.FindAll(room => booking.Rooms.Any(r => r.Id == room.Id));
        }

        private static IQueryable<Option> _getOptionRequest(ResotelContext ctx)
        {
            IQueryable<Option> optionsRequest =
                ctx.Options
                .Include(opt => opt.Rooms.Select(room => room.Options))
                .Include(opt => opt.Discounts.Select(discount => discount.Validity));
            return optionsRequest;
        }

        private static IQueryable<Room> _getRoomRequest(ResotelContext ctx, bool fromCtx)
        {
            IQueryable<Room> rooms = ctx.Rooms;

            if (!fromCtx)
            {
                rooms = ctx.Rooms.AsNoTracking();
            }
            IQueryable<Room> loadRoomsRequest = rooms
                .Include(room => room.Options.Select(o => o.Discounts))
                .Include(room => room.AvailablePacks)
                .Include(room => room.Bookings.Select(b => b.OptionChoices))
                .Include(room => room.Bookings
                    .Select(b => b.OptionChoices
                        .Select(optC => optC.Option)
                        .Select(o => o.Discounts
                            .Select(discount => discount.Validity)
                        )
                    )
                )
                .Include(room => room.Bookings.Select(b => b.Client))
                .Include(room => room.Bookings.Select(b => b.Dates))
                .Include(room => room.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room)))
                .Include(room => room.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room.Options)))
                .Include(room => room.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.RoomPack)));

            return loadRoomsRequest;
        }
    }
}
