using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{    /// <summary>
     /// ViewModel pendant of DateRange with changes notifications
     /// </summary>
    class DateRangeEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private DateRange _dateRange;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public DateRange DateRange
        {
            get { return _dateRange; }
        }

        public int Id
        {
            get { return _dateRange.Id; }
            set
            {
                _dateRange.Id = value;
                _pcs.NotifyChange();
            }
        }

        public DateTime Start
        {
            get { return _dateRange.Start; }
            set
            {
                _dateRange.Start = value;
                _pcs.NotifyChange();
            }
        }

        public DateTime End
        {
            get { return _dateRange.End; }
            set
            {
                _dateRange.End = value;
                _pcs.NotifyChange();
            }
        }

        public int Days { get { return _dateRange.Days; } }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = ((IDataErrorInfo)_dateRange)[columnName];
                if (columnName == nameof(Start) && error == null && DateTime.Now.Date.CompareTo(_dateRange.Start) > 0 && _dateRange.Id == 0)
                {
                    error = "Une nouvelle réservation ne peut être dans le passé.";
                }
                return error;
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                string error = ((IDataErrorInfo)_dateRange).Error;
                if(DateTime.Now.Date.CompareTo(_dateRange.Start) > 0 && _dateRange.Id == 0)
                {
                    error = $"{error};Une nouvelle réservation ne peut être dans le passé";
                }
                return error;
            }
        }

        

        public DateRangeEntity(DateRange dateRange)
        {
            _pcs = new PropertyChangeSupport(this);
            _dateRange = dateRange;
        }
    }
}
