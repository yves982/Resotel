using ResotelApp.ViewModels.Utils;
using System;
using System.Windows.Threading;

namespace ResotelApp.Views.Utils
{
    class UITimer : IUITimer
    {
        private DispatcherTimer _timer;

        public int IntervalMS
        {
            get { return (int)_timer.Interval.TotalMilliseconds; }

            set { _timer.Interval = TimeSpan.FromMilliseconds(value); }
        }

        public bool IsEnabled
        {
            get { return _timer.IsEnabled; }

            set { _timer.IsEnabled = value; }
        }

        public event EventHandler Elapsed
        {
            add { _timer.Tick += value; }
            remove { _timer.Tick -= value; }
        }

        public UITimer()
        {
            _timer = new DispatcherTimer();
        }
    }
}
