using ResotelApp.Models;

namespace ResotelApp.ViewModels.Utils
{
    class Tva
    {
        private static double _value;

        public static double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Booking.Tva = _value;
            }
        }
    }
}
