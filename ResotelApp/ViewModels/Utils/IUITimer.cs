using System;

namespace ResotelApp.ViewModels.Utils
{
    interface IUITimer
    {
        event EventHandler Elapsed;

        int IntervalMS { get; set; }
        bool IsEnabled { get; set; }
    }
}
