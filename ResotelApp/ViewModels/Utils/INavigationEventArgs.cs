using System;

namespace ResotelApp.ViewModels.Utils
{
    public interface INavigationEventArgs
    {
        object Data { get;}
        Uri LoadedLocation { get; }
    }
}
