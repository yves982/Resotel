using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// ViewModel pendant of PaymentEntity with changes notifications
    /// </summary>
    class PaymentEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private double _requiredAmmount;
        private Booking _booking;

        public DateTime? Date
        {
            get { return _booking.Payment.Date; }
            set
            {
                _booking.Payment.Date = value;
                _pcs.NotifyChange();
            }
        }

        public double Ammount
        {
            get { return _booking.Payment.Ammount; }
            set
            {
                _booking.Payment.Ammount = value;
                _pcs.NotifyChange();
            }
        }

        public PaymentMode Mode
        {
            get { return _booking.Payment.Mode; }
            set
            {
                _booking.Payment.Mode = value;
                _pcs.NotifyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Payment Payment
        {
            get { return _booking.Payment; }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                string error = ((IDataErrorInfo)_booking.Payment).Error;
                if(Math.Abs(_requiredAmmount - _booking.Payment.Ammount) > 0.009)
                {
                    error = $"Le montant du paiement doit être de {_requiredAmmount}";
                }
                return error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = ((IDataErrorInfo)_booking.Payment)[columnName];
                if (columnName == nameof(Ammount) && Math.Abs(_requiredAmmount - _booking.Payment.Ammount) > 0.009)
                {
                    error = $"Le montant du paiement doit être de {_requiredAmmount:0.00}";
                }
                return error;
            }
        }


        public PaymentEntity(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _booking = booking;
            _requiredAmmount = booking.Total;
            _booking.Payment = booking.Payment;
        }
    }
}
