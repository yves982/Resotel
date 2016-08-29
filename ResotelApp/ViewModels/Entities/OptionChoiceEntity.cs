using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.ViewModels.Entities
{
    class OptionChoiceEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private OptionChoice _optionChoice;
        private string _imageFullPath;
        private bool _taken;

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
                if(_optionChoice.Option.CurrentDiscount != null)
                {
                    reduceByPertcent = _optionChoice.Option.CurrentDiscount.ReduceByPercent;
                }
                return reduceByPertcent;
            }
            set
            {
                if(_optionChoice.Option.CurrentDiscount == null)
                {
                    _optionChoice.Option.CurrentDiscount = new Discount();
                }
                _optionChoice.Option.CurrentDiscount.ReduceByPercent = value;
                _pcs.NotifyChange();
            }
        }

        public double ActualPrice
        {
            get { return _optionChoice.Option.ActualPrice; }
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
            }
        }

        public DateRange TakenDates
        {
            get { return _optionChoice.TakenDates; }
            set
            {
                _optionChoice.TakenDates = value;
                _pcs.NotifyChange();
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return ((IDataErrorInfo)_optionChoice).Error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return ((IDataErrorInfo)_optionChoice)[columnName];
            }
        }

        public OptionChoiceEntity(OptionChoice optionChoice)
        {
            _pcs = new PropertyChangeSupport(this);
            _optionChoice = optionChoice;
            
            string cleanedLabel;
            
            cleanedLabel = _cleanLabel(optionChoice.Option.Label);
            _imageFullPath = string.Format("/Resources/{0}.png", cleanedLabel);
            _taken = false;
        }

        private string _cleanLabel(string optionLabel)
        {
            string[] sources = new string[3] { "é", "-", " " };
            string[] replacements = new string[3] { "e", "_", "" };

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

        private OptionChoiceEntity(string description, string imageFullName, OptionChoice option, bool taken = false)
        {
            _pcs = new PropertyChangeSupport(this);
            _optionChoice = option;
            _imageFullPath = imageFullName;
            _taken = taken;
        }
    }
}
