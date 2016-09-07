using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels.Entities
{
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
            get { return ((IDataErrorInfo)_dateRange)[columnName]; }
        }

        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_dateRange).Error; }
        }

        

        public DateRangeEntity(DateRange dateRange)
        {
            _pcs = new PropertyChangeSupport(this);
            _dateRange = dateRange;
        }
    }
}
