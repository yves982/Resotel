using ResotelApp.ViewModels.Utils;
using System;
using System.Windows.Navigation;

namespace ResotelApp.Views.Utils
{
    class NavigationFailedEventArgsImp : INavigationFailedEventArgs
    {
        private NavigationFailedEventArgs _navFailedEvent;

        public object Data
        {
            get { return _navFailedEvent.ExtraData; }
        }

        public Exception Exception
        {
            get { return _navFailedEvent.Exception; }
        }

        public Uri FailedLocation
        {
            get { return _navFailedEvent.Uri; }
        }

        public bool Handled
        {
            get { return _navFailedEvent.Handled; }
            set { _navFailedEvent.Handled = value; }
        }

        public NavigationFailedEventArgsImp(NavigationFailedEventArgs e)
        {
            _navFailedEvent = e;
        }
    }
}
