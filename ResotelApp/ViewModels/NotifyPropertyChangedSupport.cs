using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResotelApp.ViewModels
{
    class NotifyPropertyChangedSupport : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            bool success = !EqualityComparer<T>.Default.Equals(field, value);
            field = value;
            OnPropertyChanged(propertyName);
            return success;
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
