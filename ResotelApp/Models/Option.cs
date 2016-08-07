using ResotelApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace ResotelApp.Models
{
    public class Option
    {
        public int Id { get; set; }
        [Required]
        public string Label { get; set; }
        public List<Room> Rooms { get; set; }
        [Required]
        public double BasePrice { get; set; }
        public Discount CurrentDiscount { get; set; }

        public Option()
        {
            Rooms = new List<Room>();
            CurrentDiscount = new Discount();
        }

        private static Expression<Func<Option,bool>> _noBookedRoomDuring(DateRange dateRange)
        {
            return opt => opt.Rooms.AsQueryable().Any(Room.NotBookedDuring(dateRange));
        }

        public static Expression<Func<Option,bool>> IsAvailableBetween(DateRange dateRange)
        {
            Expression<Func<Option, bool>> noRooms = opt => opt.Rooms.Count == 0;
            return noRooms
            .Or(_noBookedRoomDuring(dateRange));
        }
    }
}