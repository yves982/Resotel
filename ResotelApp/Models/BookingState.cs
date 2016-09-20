namespace ResotelApp.Models
{
    public enum BookingState
    {
        /// <summary>Indicates a Booking has been approved, a newly saved Booking is Validated</summary>
        Validated,
        /// <summary>Indicated a Booking has been paid (there's no partial payment, but solving the bill can be delayed)</summary>
        Paid,
        /// <summary>Indicates a Booking has been Cancelled but without refunds. This can be on client request or due to any other circumstance resulting in premature end.</summary>
        Cancelled,
        /// <summary>Indicated a Booking cancelled on client's request, fully refunded as the 48h delay has been followed</summary>
        FullyCancelled
    }
}
