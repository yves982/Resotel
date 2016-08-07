using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    public class Discount
    {
        public int Id { get; set; }
        [Required]
        public double ReduceByPercent { get; set; }
        public int ApplicableQuantity { get; set; }
        public DateRange Validity { get; set; }
    }
}