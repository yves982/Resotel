using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels.Utils
{
    interface INavigationEventArgs
    {
        object Data { get;}
        Uri LoadedLocation { get; }
    }
}
