using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    class Client
    {
        public int ClientId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Address { get; set; }
        public List<Booking> Bookings { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
