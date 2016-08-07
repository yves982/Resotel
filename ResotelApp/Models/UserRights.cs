using System;

namespace ResotelApp.Models
{
    [Flags]
    enum UserRights
    {
        Booking = 1,
        Cleaning = 2,
        Invoicing = 4
    }
}
