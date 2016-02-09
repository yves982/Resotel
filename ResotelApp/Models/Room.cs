

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResotelApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        [Required]
        public int Stage { get; set; }
        [Required]
        [DefaultValue(1)]
        public int Size { get; set; }
        public List<Option> Options { get; set; }
        public Booking Booking { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsCleaned { get; set; }
    }
}
