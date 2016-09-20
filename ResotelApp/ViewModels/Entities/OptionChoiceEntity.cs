using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// ViewModel pendant of OptionChoice with changes notifications
    /// </summary>
    class OptionChoiceEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private OptionChoice _optionChoice;
        private string _imageFullPath;
        private bool _taken;
        private Booking _booking;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public OptionChoice OptionChoice
        {
            get { return _optionChoice; }
        }

        public int PeopleCount
        {
            get { return _optionChoice.PeopleCount; }
            set
            {
                _optionChoice.PeopleCount = value;
                _pcs.NotifyChange();
            }
        }

        public string Description
        {
            get { return _optionChoice.Option.Label; }

            set
            {
                _optionChoice.Option.Label = value;
                _pcs.NotifyChange();
            }
        }

        public double BasePrice
        {
            get { return _optionChoice.Option.BasePrice; }
            set
            {
                _optionChoice.Option.BasePrice = value;
                _pcs.NotifyChange();
            }
        }

        public double ReduceByPercent
        {
            get
            {
                double reduceByPertcent = 0d;
                if (_optionChoice.Option.CurrentDiscount != null)
                {
                    reduceByPertcent = _optionChoice.Option.CurrentDiscount.ReduceByPercent;
                }
                return reduceByPertcent;
            }
            set
            {
                _optionChoice.Option.CurrentDiscount.ReduceByPercent = value;
                _pcs.NotifyChange();
            }
        }

        public double ActualPrice
        {
            get { return _optionChoice.ActualPrice; }
        }

        public bool IsPeopleRelated
        {
            get { return _optionChoice.Option.Label.Equals("Restauration"); }
        }

        public string ImageFullPath
        {
            get { return _imageFullPath; }

            set
            {
                _imageFullPath = value;
                _pcs.NotifyChange();
            }
        }

        public bool Taken
        {
            get { return _taken; }

            set
            {
                _taken = value;
                _pcs.NotifyChange();
                _pcs.NotifyChange(nameof(DateChoiceEnabled));
                _pcs.NotifyChange(nameof(HasActiveFixedDates));
            }
        }

        public bool DateChoiceEnabled
        {
            get { return _optionChoice.CanChangeDates && _taken; }
        }

        public bool HasActiveFixedDates
        {
            get { return !_optionChoice.CanChangeDates && _taken; }
        }

        public DateTime TakenStart
        {
            get { return _optionChoice.TakenDates.Start; }
            set
            {
                _optionChoice.TakenDates.Start = value;
                _pcs.NotifyChange();
            }
        }

        public DateTime TakenEnd
        {
            get { return _optionChoice.TakenDates.End; }
            set
            {
                _optionChoice.TakenDates.End = value;
                _pcs.NotifyChange();
            }
        }

        public DateRange DiscountedDates
        {
            get
            {
                DateRange discountedDates = null;
                if (_optionChoice.Option.CurrentDiscount != null && _optionChoice.Option.CurrentDiscount.Validity != null)
                {
                    discountedDates = _optionChoice.Option.CurrentDiscount.Validity;
                }
                return discountedDates;
            }
        }

        public bool HasPartialDiscount
        {
            get
            {
                bool hasPartialDiscount = false;
                if (_optionChoice.Option.CurrentDiscount != null)
                {
                    Discount currentDiscount = _optionChoice.Option.CurrentDiscount;
                    DateTime takenStart = _optionChoice.TakenDates.Start;
                    DateTime takenEnd = _optionChoice.TakenDates.End;
                    if (DiscountedDates != null &&
                        (!DiscountedDates.Start.Date.Equals(takenStart.Date)) || !DiscountedDates.End.Date.Equals(takenEnd.Date))
                    {
                        hasPartialDiscount = true;
                    }
                }
                return hasPartialDiscount;
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                string error = ((IDataErrorInfo)_optionChoice).Error;
                // Restauration
                if (_optionChoice.Option.Id == 8
                    && _optionChoice.TakenDates.Start.Subtract(DateTime.Now.Date).TotalDays < 1.0d
                    && Taken
                )
                {
                    error = error == null ? "Les repas doivent être commandés au moins 24H en avance" :
                        error + ";Les repas doivent être commandés au moins 24H en avance";
                }
                else if (
                    (_optionChoice.TakenDates.End.CompareTo(_booking.Dates.Start) < 0
                   || _optionChoice.TakenDates.Start.CompareTo(_booking.Dates.End) > 0)
                   )
                {
                    error = error == null ? "La date de début n'est pas comprise dans l'intervalle de temps de la réservation"
                        : error + ";La date de début n'est pas comprise dans l'intervalle de temps de la réservation";
                }
                return error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = ((IDataErrorInfo)_optionChoice)[columnName];
                if (_optionChoice.Option.Id == 8
                    && _optionChoice.TakenDates.Start.Subtract(DateTime.Now.Date).TotalDays < 1.0d
                    && (columnName == nameof(TakenStart)
                    && Taken)
                )
                {
                    error = "Les repas doivent être commandés au moins 24H en avance";
                }
                else if (
                    (_optionChoice.TakenDates.Start.CompareTo(_booking.Dates.Start) < 0
                   || _optionChoice.TakenDates.End.CompareTo(_booking.Dates.End) > 0)
                   && ( columnName == nameof(TakenStart) || columnName == nameof(TakenEnd))
                   )
                {
                    error = "Les dates de début et de fin l'option doivent être comprises dans l'intervalle de temps de la réservation";

                }

                
                return error;
            }
        }

        public OptionChoiceEntity(Booking booking, OptionChoice optionChoice)
        {
            _pcs = new PropertyChangeSupport(this);
            _optionChoice = optionChoice;
            _booking = booking;
            

            string cleanedLabel;
            if (optionChoice != null)
            {
                cleanedLabel = _cleanLabel(optionChoice.Option.Label);
                _imageFullPath = string.Format("/Resources/{0}.png", cleanedLabel);
                
                // restauration
                if(optionChoice.PeopleCount == 0 && optionChoice.Option.Id == 8)
                {
                    PeopleCount = 1;
                }
            }
            _taken = false;
        }

        private string _cleanLabel(string optionLabel)
        {
            string[] sources = new string[4] { "é", "-", " " , "/"};
            string[] replacements = new string[4] { "e", "_", "", "_" };

            string cleanedLabel;
            StringBuilder sb = new StringBuilder(optionLabel.Length);

            for (int i = 0; i < optionLabel.Length; i++)
            {
                int sourcesIndex = Array.IndexOf(sources, optionLabel[i].ToString());
                if (sourcesIndex != -1)
                {
                    sb.Append(replacements[sourcesIndex]);
                }
                else
                {
                    sb.Append(optionLabel[i]);
                }
            }
            cleanedLabel = sb.ToString();
            return cleanedLabel;
        }
    }
}
