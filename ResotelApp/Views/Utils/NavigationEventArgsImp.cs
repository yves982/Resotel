using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
