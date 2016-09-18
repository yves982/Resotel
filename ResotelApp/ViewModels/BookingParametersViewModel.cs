using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
            get { return _validateCommand; }
        }

        public BookingParametersViewModel(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            
            booking.Dates.Start = booking.Dates.Start.Date;
            booking.Dates.End = booking.Dates.End.Date;
            _dateRange = new DateRangeEntity(booking.Dates);
            _dateRange.PropertyChanged += _dateRange_PropertyChanged;
            _adultsCount = booking.AdultsCount;
            _babiesCount = booking.BabiesCount;
            _validateCommand = new DelegateCommand<object>(_validate);
        }

        public void ChangeValidateCanExecute()
        {
            bool validatesDateRange = _validateDateRange() == null;
            if (validatesDateRange)
            {
                _validateCommand.ChangeCanExecute();
            }
        }

        private string _validateDateRange()
        {
            string error = ((IDataErrorInfo)_dateRange).Error;
            return error;
        }
        
        private void _dateRange_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _unlockValidationIfNeeded();
            _pcs.NotifyChange(nameof(DateRangeEntity));
        }

        private void _unlockValidationIfNeeded()
        {
            bool canValidate = _validateCommand.CanExecute(null);
            bool validatesDateRange = _validateDateRange() == null;

            if(
                (!canValidate && validatesDateRange) ||
                (canValidate && !validatesDateRange)
            )
            {
                _validateCommand.ChangeCanExecute();
            }
        }

        private void _validate(object ignore)
        {
            _validateCommand.ChangeCanExecute();
            Defined?.Invoke(null, this);
        }
    }
}
