using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Xps.Packaging;

namespace ResotelApp.ViewModels
{
    class SumUpViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private string _title;
        private LinkedList<INavigableViewModel> _navigation;
        private XpsDocument _xpsDoc;

        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }

        public string Title
        {
            get { return _title; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public SumUpViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
        }
    }
}
