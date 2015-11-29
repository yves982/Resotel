using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    public class Option
    {
        public int OptionId { get; set; }
        [Required]
        public string Label { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsRoomRelated { get; set; }
        [Required]
        public int BasePrice { get; set; }
    }
}