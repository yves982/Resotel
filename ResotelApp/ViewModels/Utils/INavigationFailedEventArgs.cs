using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels.Utils
{
    interface INavigationFailedEventArgs
    {
        Exception Exception { get;  }
        bool Handled { get; set; }
        object Data { get;  }
        Uri FailedLocation { get; }
    }
}
