using ResotelApp.ViewModels.Utils;
using System;
using System.Windows.Navigation;

namespace ResotelApp.Views.Utils
{
    class NavigationEventArgsImp : INavigationEventArgs
    {
        private NavigationEventArgs _navEvent;

        public NavigationEventArgsImp(NavigationEventArgs e)
        {
            this._navEvent = e;
        }

        public object Data
        {
            get { return _navEvent.ExtraData; }
        }

        public Uri LoadedLocation
        {
            get { return _navEvent.Uri; }
        }   
        
    }
}
