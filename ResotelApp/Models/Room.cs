using ResotelApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace ResotelApp.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public int Stage { get; set; }
        [Required]
        [DefaultValue(1)]
        public int Size { get; set; }
        public List<Option> Options { get; set; }
        public List<Booking> Bookings { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsCleaned { get; set; }
        public BedKind BedKind { get; set; }

        public static Expression<Func<Room,bool>> _hasNoBooking()
        {
            return room => room.Bookings.Count == 0;
        }

        private static Expression<Func<Room, bool>> _bookingsStartedAfter(DateTime start)
        {
            return room => !room.Bookings.Any(booking => booking.Dates.Start < start);
        } 

        private static Expression<Func<Room, bool>> _bookingsEndedBefore(DateTime end)
        {
            return room => !room.Bookings.Any(booking => booking.Dates.End > end);
        }

        public static Expression<Func<Room, bool>> NotCurrentlyBooked()
        {
            return _hasNoBooking()
                .Or(_bookingsStartedAfter(DateTime.Now))
                .Or(_bookingsEndedBefore(DateTime.Now));
        }

        public static Expression<Func<Room, bool>> NotBookedDuring(DateRange dateRange)
        {
            return _hasNoBooking()
                .Or(_bookingsStartedAfter(dateRange.Start))
                .Or(_bookingsEndedBefore(dateRange.End));
        }

        public static Expression<Func<Room, bool>> WithBedKind(BedKind bedKind)
        {
            return room => room.BedKind == bedKind;
        }

        public static Expression<Func<Room, bool>> WithOptions(IEnumerable<Option> seekedOptions)
        {
            return room => !seekedOptions.Any(seekedOpt => !room.Options.Contains(seekedOpt));
        }

        public Room()
        {
            Options = new List<Option>();
            Bookings = new List<Booking>();
        }
    }
}
