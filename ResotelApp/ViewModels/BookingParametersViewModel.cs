using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class BookingParametersViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private DateRange _dateRange;
        private int _babiesCount;
        private int _adultsCount;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public DateRange DateRange
        {
            get { return _dateRange; }
            set
            {
                _dateRange = value;
                _pcs.NotifyChange();
            }
        }

        public int BabiesCount
        {
            get { return _babiesCount; }

            set
            {
                _babiesCount = value;
                _pcs.NotifyChange();
            }
        }

        public int AdultsCount
        {
            get { return _adultsCount; }

            set
            {
                _adultsCount = value;
                _pcs.NotifyChange();
            }
        }

        public BookingParametersViewModel(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _dateRange = booking.Dates;
            _adultsCount = booking.AdultsCount;
            _babiesCount = booking.BabiesCount;
        }
    }
}
