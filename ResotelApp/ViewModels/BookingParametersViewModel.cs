using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class BookingParametersViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private DateRangeEntity _dateRange;
        private int _babiesCount;
        private int _adultsCount;

        private DelegateCommand<object> _validateCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<BookingParametersViewModel> Defined;

        public DateRangeEntity DateRangeEntity
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
            _dateRange = new DateRangeEntity(booking.Dates);
            _dateRange.PropertyChanged += _dateRange_PropertyChanged;
            _adultsCount = booking.AdultsCount;
            _babiesCount = booking.BabiesCount;
        }

        public void ChangeValidateCanExecute()
        {
            if (!_validateCommand.CanExecute(null))
            {
                _validateCommand.ChangeCanExecute();
            }
        }

        private void _dateRange_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _pcs.NotifyChange(nameof(DateRangeEntity));
        }


        private void _validate(object ignore)
        {
            _validateCommand.ChangeCanExecute();
            Defined?.Invoke(null, this);
        }
    }
}
