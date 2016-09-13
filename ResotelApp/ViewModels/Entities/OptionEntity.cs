using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;
using System;

namespace ResotelApp.ViewModels.Entities
{
    class OptionEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private Option _opt;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Option Option { get { return _opt; } }

        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_opt).Error; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ((IDataErrorInfo)_opt)[columnName]; }
        }

        public OptionEntity(Option opt)
        {
            _pcs = new PropertyChangeSupport(this);
            _opt = opt;
        }

        
    }
}