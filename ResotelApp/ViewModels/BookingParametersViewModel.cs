using ResotelApp.Models;
using ResotelApp.Utils;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class BookingParametersViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private DateRangeEntity _dateRange;
        private int _babiesCount;
        private int _adultsCount;

        private DelegateCommand<object> _validateCommand;
        private Booking _booking;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<BookingParametersViewModel> Defined;

        public DateTime Start
        {
            get { return _dateRange.Start; }
            set
            {
                _dateRange.Start = value;
                _booking.Dates.Start = _dateRange.Start.Date;
                _unlockValidationIfNeeded();
                _pcs.NotifyChange();
            }
        }

        public DateTime End
        {
            get { return _dateRange.End; }
            set
            {
                _dateRange.End = value;
                _booking.Dates.End = _dateRange.End.Date;
                _unlockValidationIfNeeded();
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

        string IDataErrorInfo.Error
        {
            get
            {
                string error = ((IDataErrorInfo)_dateRange).Error;
                if(_booking.Id > 0 && Start.Date.Subtract(DateTime.Now.Date).TotalDays < 2.0d )
                {
                    error = error == null ? "La réservation ne peut être modifiée moins de 24H en avance" 
                        : $"{error};La réservation ne peut être modifiée moins de 24H en avance";
                }
                return error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = ((IDataErrorInfo)_dateRange)[columnName];
                if (_booking.Id > 0 && Start.Date.Subtract(DateTime.Now.Date).TotalDays < 2.0d
                    && (columnName == nameof(Start))
                )
                {
                    error = error == null ? "La réservation ne peut être modifiée moins de 24H en avance"
                        : $"{error};La réservation ne peut être modifiée moins de 24H en avance";
                }
                return error;
            }
        }

        public BookingParametersViewModel(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);

            _booking = booking;
            _booking.Dates.Start = booking.Dates.Start.Date;
            _booking.Dates.End = booking.Dates.End.Date;
            _dateRange = new DateRangeEntity(booking.Dates);
            _adultsCount = booking.AdultsCount;
            _babiesCount = booking.BabiesCount;
            _validateCommand = new DelegateCommand<object>(_validate);

            if(_adultsCount == 0 && booking.Id == 0)
            {
                AdultsCount = 1;
            }
        }

        public void ChangeValidateCanExecute()
        {
            try
            {
                bool validatesDateRange = _validateDateRange() == null;
                if (validatesDateRange)
                {
                    _validateCommand.ChangeCanExecute();
                }
            }
            catch (Exception ex)
            {

                Logger.Log(ex);
            }
        }

        private string _validateDateRange()
        {
            string error = ((IDataErrorInfo)_dateRange).Error;
            return error;
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
            try
            {
                _validateCommand.ChangeCanExecute();
                Defined?.Invoke(null, this);
            }
            catch (Exception ex)
            {

                Logger.Log(ex);
            }
        }
    }
}
