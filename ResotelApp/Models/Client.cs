﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    public class Client
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
    }
}
