using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResotelApp.ViewModels.Utils
{
    class PropertyChangeSupport
    {
        public static void NotifyChange(object sender, PropertyChangedEventHandler propertyChanged, [CallerMemberName]string propertyName = null)
        {
            if(propertyChanged != null)
            {
                propertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
