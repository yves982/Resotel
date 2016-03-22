using System;

namespace ResotelApp.ViewModels.Utils
{
    interface INavigationService
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        Uri Source { get; }

        event Action<object, INavigationFailedEventArgs> NavigationFailed;
        event Action<object, INavigationEventArgs> NavigationStopped;
        event Action<object, INavigationEventArgs> LoadCompleted;

        void GoBack();
        void GoForward();
        void Navigate(Uri uri);
        void Navigate(Uri uri, object data);
        void Refresh();
        void StopLoading();

    }
}
