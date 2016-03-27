using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    class Booking
    {
        public int BookingId { get; set; }
        [Required]
        public Client Client { get; set; }
        public List<Option> Options { get; set; }
        public DateTime Date { get; set; }
        public Discount CurrentDiscount { get; set; }
        public List<Room> Rooms { get; set; }
        public DateRange Dates { get; set; }
        public int AdultsCount { get;set; }
        public int BabiesCount { get; set; }
    }
}
