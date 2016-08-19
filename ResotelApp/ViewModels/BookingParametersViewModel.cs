using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class BookingParametersViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private DateRange _dateRange;
        private int _babiesCount;
        private int _adultsCount;

        private DelegateCommand<object> _validateCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<BookingParametersViewModel> Defined;

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
                if (!_validateCommand.CanExecute(null))
                {
                    _validateCommand.ChangeCanExecute();
                }
                _pcs.NotifyChange();
            }
        }

        public int AdultsCount
        {
            get { return _adultsCount; }

            set
            {
                _adultsCount = value;
                if(!_validateCommand.CanExecute(null))
                {
                    _validateCommand.ChangeCanExecute();
                }
                _pcs.NotifyChange();
            }
        }

        public ICommand ValidateCommand
        {
            get
            {
                if(_validateCommand == null)
                {
                    _validateCommand = new DelegateCommand<object>(_validate);
                }
                return _validateCommand;
            }
        }

        public BookingParametersViewModel(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _dateRange = booking.Dates;
            _adultsCount = booking.AdultsCount;
            _babiesCount = booking.BabiesCount;
        }

        private void _validate(object ignore)
        {
            _validateCommand.ChangeCanExecute();
            if(Defined != null)
            {
                Defined(null, this);
            }
        }
    }
}
