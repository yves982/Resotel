using ResotelApp.Models;
using ResotelApp.Models.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ResotelApp.DAL
{
    class BookingRepository
    {

        public async static Task<Booking> Save(Booking booking)
        {
            using (ResotelContext ctx = new ResotelContext())
            {
                Booking savedBooking = null;

                List<Room> rooms = await _getRoomRequest(ctx, true)
                        .ToListAsync();
                List<Option> options = await _getOptionRequest(ctx)
                    .ToListAsync();

                Client client = await _getClientRequest(ctx).FirstOrDefaultAsync();

                _assignCtxRooms(booking, rooms);
                _assignCtxOptions(booking, options);
                booking.Client = client;

                if (booking.Id == 0)
                {

                    ctx.Entry(booking).State = EntityState.Added;
                }
                else
                {
                    Booking trackedBooking = await ctx.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
                    ctx.Entry(trackedBooking).State = EntityState.Modified;
                    ctx.Entry(trackedBooking).CurrentValues.SetValues(booking);

                    for (int i = trackedBooking.OptionChoices.Count - 1; i >= 0; i--)
                    {
                        ctx.Entry(trackedBooking.OptionChoices[i]).State = EntityState.Deleted;
                    }
                    // the joy of using an ORM to manually update a recursive object graph ... priceless
                    foreach (OptionChoice optChoice in booking.OptionChoices)
                    {
                        OptionChoice trackedOptChoice = await ctx.Set<OptionChoice>().FirstOrDefaultAsync(optC => optC.Id == optChoice.Id);
                        if (trackedOptChoice == null)
                        {
                            trackedOptChoice = ctx.Set<OptionChoice>().Add(optChoice);

                        }

                        Option trackedOpt = await ctx.Options.FirstOrDefaultAsync(opt => opt.Id == optChoice.Id);


                        DateRange trackedOptDates = await ctx.Set<DateRange>().FirstOrDefaultAsync(dr => dr.Id == optChoice.TakenDates.Id);
                        if (trackedOptDates == null)
                        {
                            trackedOptDates = ctx.Set<DateRange>().Add(optChoice.TakenDates);
                        }
                        ctx.Entry(trackedOptDates).CurrentValues.SetValues(optChoice.TakenDates);

                        ctx.Entry(trackedOptChoice).CurrentValues.SetValues(optChoice);
                        trackedBooking.OptionChoices.Add(trackedOptChoice);
                    }

                    trackedBooking.Rooms.Clear();
                    foreach (Room room in booking.Rooms)
                    {
                        Room trackedRoom = await ctx.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);
                        ctx.Entry(trackedBooking).Collection(bking => bking.Rooms).EntityEntry.State = EntityState.Modified;
                        trackedBooking.Rooms.Add(trackedRoom);
                    }

                    trackedBooking.RoomPacks.Clear();
                    foreach (AppliedPack appliedPack in booking.RoomPacks)
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
                    }

                    ctx.Entry(trackedBooking).Property(b => b.State).EntityEntry.State = EntityState.Modified;
                    ctx.Entry(trackedBooking).Property(b => b.State).EntityEntry.CurrentValues.SetValues(booking.State);

                    DateRange trackedBookingDates = await ctx.Set<DateRange>().FirstOrDefaultAsync(dr => dr.Id == booking.Dates.Id);
                    if (trackedBookingDates == null)
                    {
                        trackedBookingDates = ctx.Set<DateRange>().Attach(booking.Dates);
                    }
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

        private static IQueryable<Client> _getClientRequest(ResotelContext ctx)
        {
            IQueryable<Client> clientRequest = ctx.Clients
                                .Include(cl => cl.Bookings)
                                .Include(cl => cl.Bookings.Select(b => b.Rooms))
                                .Include(cl => cl.Bookings.Select(b => b.Rooms.Select( r => r.Options )))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room)))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.Room.Options)))
                                .Include(cl => cl.Bookings.Select(b => b.RoomPacks.Select(appliedPack => appliedPack.RoomPack)))
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
