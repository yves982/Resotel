using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.Models
{
    class Booking
    {
        public int BookingId { get; set; }
        public Client Client { get; set; }
        public List<Option> Options { get; set; }
        public DateTime Date { get; set; }
        public Discount CurrentDiscount { get; set; }

    }
}
