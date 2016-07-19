using System.ComponentModel;
using ResotelApp.ViewModels.Utils;

namespace ResotelApp.ViewModels
{
    class OptionsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }


        public OptionsViewModel()
        {
            _pcs = new Utils.PropertyChangeSupport(this);
        }
    }
}
