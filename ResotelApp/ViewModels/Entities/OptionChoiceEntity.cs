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
        private string _description;
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

        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                _pcs.NotifyChange();
            }
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
            _description = optionChoice.Option.Label;
            
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
            _description = description;
            _imageFullPath = imageFullName;
            _taken = taken;
        }
    }
}
