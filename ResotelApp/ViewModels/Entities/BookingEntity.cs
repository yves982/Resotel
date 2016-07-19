using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class BookingEntity : IEntity, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private Booking _booking;
        private string _title;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Booking Booking
        {
            get { return _booking; }
            set
            {
                _booking = value;
                _pcs.NotifyChange();
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _pcs.NotifyChange();
            }
        }

        public BookingEntity(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            Booking = booking;
        }
    }
}
