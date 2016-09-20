using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResotelApp.ViewModels.Utils
{
    /// <summary>
    /// Helper class to handle INotfyPropertyChange
    /// used with event delegation on Handler
    /// </summary>
    class PropertyChangeSupport
    {
        private object _sender;
        private PropertyChangedEventHandler _handler;

        public PropertyChangedEventHandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public PropertyChangeSupport(object sender)
        {
            _sender = sender;
        }

        public void NotifyChange([CallerMemberName]string propertyName = null)
        {
            _handler?.Invoke(_sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
