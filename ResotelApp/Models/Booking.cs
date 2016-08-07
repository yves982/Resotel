using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        public Client Client { get; set; }
        public List<Option> Options { get; set; }
        public DateTime CreationDate { get; set; }
        public Discount CurrentDiscount { get; set; }
        public List<Room> Rooms { get; set; }
        public DateRange Dates { get; set; }
        public int AdultsCount { get;set; }
        public int BabiesCount { get; set; }

        public Booking()
        {
            Client = new Client();
            Options = new List<Option>();
            CurrentDiscount = new Discount();
            Rooms = new List<Room>();
            Dates = new DateRange();
        }

        public void AddClient(Client client)
        {
            Client = client;
            client.Bookings.Add(this);
        }
    }
}
