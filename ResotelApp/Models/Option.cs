using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    class Option
    {
        public int OptionId { get; set; }
        [Required]
        public string Label { get; set; }
        public List<Room> Rooms { get; set; }
        [Required]
        public int BasePrice { get; set; }
        public Discount CurrentDiscount { get; set; }
    }
}