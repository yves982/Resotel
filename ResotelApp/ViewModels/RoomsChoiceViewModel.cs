using System.ComponentModel;
using ResotelApp.ViewModels.Utils;

namespace ResotelApp.ViewModels
{
    class RoomsChoiceViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public RoomsChoiceViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
        }
    }
}
