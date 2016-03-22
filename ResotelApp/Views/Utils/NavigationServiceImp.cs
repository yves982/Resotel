using ResotelApp.ViewModels.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ResotelApp.Views.Utils
{
    public class NavigationServiceImp : INavigationService
    {
        private NavigationService _navService;

        public bool CanGoBack
        {
            get { return _navService.CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return _navService.CanGoForward; ; }
        }

        public Uri Source
        {
            get { return _navService.Source; }
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

        public NavigationServiceImp(Page page)
        {
            page.Loaded += _pageLoaded;
            page.Unloaded += _pageUnloaded;
        }

        private void _pageLoaded(object sender, RoutedEventArgs e)
        {
            _navService = (sender as Page).NavigationService;
            if (_navService == null)
            {
                throw new ArgumentException("L'argument doit être un NavigationHost.", "page");
            }
            _navService.LoadCompleted += frameLoaded;
            _navService.NavigationFailed += navigationFailed;
            _navService.NavigationStopped += navigationStopped;
        }

        private void _pageUnloaded(object sender, RoutedEventArgs e)
        {
            if(sender as Page != null)
            {
                Page p = sender as Page;
                p.Loaded -= _pageLoaded;
                p.Unloaded -= _pageUnloaded;
            }
        }

        public void GoBack()
        {
            _navService.GoBack();
        }

        public void GoForward()
        {
            _navService.GoForward();
        }

        public void Navigate(Uri uri)
        {
            _navService.Navigate(uri);
        }

        public void Navigate(Uri uri, object data)
        {
            _navService.Navigate(uri, data);
        }

        public void Refresh()
        {
            _navService.Refresh();
        }

        public void StopLoading()
        {
            _navService.StopLoading();
        }
    }
}
