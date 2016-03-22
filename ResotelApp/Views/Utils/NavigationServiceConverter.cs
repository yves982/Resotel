using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ResotelApp.Views.Utils
{
    [ValueConversion(typeof(Page), typeof(NavigationServiceImp))]
    public class PageToNavigationServiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NavigationServiceImp(value as Page);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
