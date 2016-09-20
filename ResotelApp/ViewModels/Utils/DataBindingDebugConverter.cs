using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ResotelApp.ViewModels.Utils
{
    /// <summary>
    /// Class used to debug XAML databindings
    /// Quite an handy thing, but often detailed enough logs are sufficient to that end.
    /// </summary>
    public class DatabindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

    }
}
