using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    class Discount
    {
        public int DiscountId { get; set; }
        [Required]
        public double ReduceByPercent { get; set; }
        public int ApplicableQuantity { get; set; }
        public DateRange Validity { get; set; }
    }
}