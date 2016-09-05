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
                await ctx.Bookings
                    .Include(bking => bking.OptionChoices.Select(optChoice => optChoice.TakenDates))
                    .Include(bking => bking.OptionChoices.Select(optChoice => optChoice.Option))
                    .Include(bking => bking.RoomPacks.Select(appliedPack => appliedPack.Room))
                    .Include(bking => bking.RoomPacks.Select(appliedPack => appliedPack.RoomPack))
                    .Include(bking => bking.Rooms.Select(room => room.Options.Select(opt => opt.Discounts)))
                    .LoadAsync();

                await ctx.Options
                    .Include(opt => opt.Discounts)
                    .Include(opt => opt.Rooms)
                    .LoadAsync();

                ctx.Bookings.Add(booking);

                List<Room> unavailableRooms = new List<Room>();
                List<Room> noAvailableReplacementRooms = new List<Room>();

                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (Room Room in booking.Rooms)
                        {
                            Room room = await ctx.Rooms
                                .AsNoTracking()
                                .FirstOrDefaultAsync(Room.NotCurrentlyBooked());
                            if (room == null)
                            {
                                unavailableRooms.Add(room);
                            }
                        }

                        if(unavailableRooms.Count == 0)
                        {
                            await ctx.SaveChangesAsync();
                            transaction.Commit();
                            savedBooking = booking;
                        }
                        else
                        {
                            foreach(Room unavailableRoom in unavailableRooms)
                            {
                                Room replacementRoom = await ctx.Rooms
                                    .AsNoTracking()
                                    .Include(room => room.AvailablePacks)
                                    .Include(room => room.Bookings)
                                    .Include(room => room.Options)
                                    .FirstOrDefaultAsync(room => room.Kind == unavailableRoom.Kind);

                                if (replacementRoom != null)
                                {
                                    booking.Rooms.Add(replacementRoom);
                                    booking.Rooms.Remove(unavailableRoom);
                                } else
                                {
                                    noAvailableReplacementRooms.Add(unavailableRoom);
                                }
                            }

                            if(noAvailableReplacementRooms.Count > 0)
                            {
                                HashSet<RoomKind> unreplaceableRoomKinds = new HashSet<RoomKind>(noAvailableReplacementRooms.ConvertAll(room => room.Kind));
                                transaction.Rollback();
                                throw new InvalidOperationException(
                                    $"Impossible de sauvegarder la réservation : nous n'avons plus assez de chambres des types {string.Join(",", unreplaceableRoomKinds)}");
                            } else
                            {
                                await ctx.SaveChangesAsync();
                                transaction.Commit();
                                savedBooking = booking;
                            }
                        }

                        return savedBooking;
                    } catch(Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
