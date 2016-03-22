using System;

namespace ResotelApp.ViewModels.Utils
{
    public interface INavigationFailedEventArgs
    {
        Exception Exception { get;  }
        bool Handled { get; set; }
        object Data { get;  }
        Uri FailedLocation { get; }
    }
}
