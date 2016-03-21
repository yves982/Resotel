using ResotelApp.ViewModels.Utils;
using ResotelApp.Views.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ResotelApp.Views
{
    class NavigationServiceImp : INavigationService
    {
        private Frame _frame;

        public bool CanGoBack
        {
            get { return _frame.NavigationService.CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return _frame.NavigationService.CanGoForward; ; }
        }

        public Uri Source
        {
            get { return _frame.NavigationService.Source; }
        }

        public event Action<object, INavigationEventArgs> LoadCompleted;
        public event Action<object, INavigationFailedEventArgs> NavigationFailed;
        public event Action<object, INavigationEventArgs> NavigationStopped;

        private void navigationStopped(object sender, NavigationEventArgs e)
        {
            if (NavigationStopped != null)
            {
                NavigationEventArgsImp navEvent = new NavigationEventArgsImp(e);
                NavigationStopped(sender, navEvent);
            }
        }

        private void navigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (NavigationFailed != null)
            {
                NavigationFailedEventArgsImp navFailedEvent = new NavigationFailedEventArgsImp(e);
            }
        }

        private void frameLoaded(object sender, NavigationEventArgs e)
        {
            if(LoadCompleted!=null)
            {
                NavigationEventArgsImp navEvent = new NavigationEventArgsImp(e);
                LoadCompleted(sender, navEvent);
            }
        }

        public NavigationServiceImp(Frame frame)
        {
            _frame = frame;
            _frame.NavigationService.LoadCompleted += frameLoaded;
            _frame.NavigationService.NavigationFailed += navigationFailed;
            _frame.NavigationService.NavigationStopped += navigationStopped;
        }

        public void GoBack()
        {
            _frame.NavigationService.GoBack();
        }

        public void GoForward()
        {
            _frame.NavigationService.GoForward();
        }

        public void Navigate(Uri uri)
        {
            _frame.NavigationService.Navigate(uri);
        }

        public void Navigate(Uri uri, object data)
        {
            _frame.NavigationService.Navigate(uri, data);
        }

        public void Refresh()
        {
            _frame.NavigationService.Refresh();
        }

        public void StopLoading()
        {
            _frame.NavigationService.StopLoading();
        }
    }
}
