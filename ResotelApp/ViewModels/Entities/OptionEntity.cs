﻿using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.ViewModels.Entities
{
    class OptionEntity : IEntity, INotifyPropertyChanged, ICloneable
    {
        private PropertyChangeSupport _pcs;
        private Option _option;
        private string _description;
        private string _imageFullPath;
        private bool _taken;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Option Option
        {
            get { return _option; }
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

        public OptionEntity(Option option)
        {
            _pcs = new PropertyChangeSupport(this);
            _option = option;
            _description = option.Label;
            string cleanedLabel;
            
            cleanedLabel = _cleanLabel(option.Label);
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

        private OptionEntity(string description, string imageFullName, Option option, bool taken = false)
        {
            _pcs = new PropertyChangeSupport(this);
            _option = option;
            _description = description;
            _imageFullPath = imageFullName;
            _taken = taken;
        }

        public object Clone()
        {
            OptionEntity option = new OptionEntity(_description, _imageFullPath, _option, _taken);
            return option;
        }
    }
}
