using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;
using System;

namespace ResotelApp.ViewModels.Entities
{
    class DiscountEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private Discount _discount;
        private DateRangeEntity _validityEntity;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public int Id
        {
            get { return _discount.Id; }
            set
            {
                _discount.Id = value;
                _pcs.NotifyChange();
            }
        }

        public double ReduceByPercent
        {
            get { return _discount.ReduceByPercent; }
            set
            {
                _discount.ReduceByPercent = value;
                _pcs.NotifyChange();
            }
        }

        public DateRangeEntity Validity
        {
            get { return _validityEntity; }
            set
            {
                _validityEntity = value;
                _discount.Validity = _validityEntity.DateRange;
                _pcs.NotifyChange();
            }
        }

        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_discount).Error; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ((IDataErrorInfo)_discount)[columnName]; }
        }


        public DiscountEntity(Discount discount)
        {
            _pcs = new PropertyChangeSupport(this);
            _discount = discount;
            if (discount != null)
            {
                _validityEntity = new DateRangeEntity(discount.Validity);
            }
        }
    }
}