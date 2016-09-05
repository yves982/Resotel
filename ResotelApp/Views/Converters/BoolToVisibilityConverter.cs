using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResotelApp.Views.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( !(value is bool) )
            {
                throw new InvalidOperationException("Seuls les booleens peuvent êtres convertis en visibilité (BoolToVisibilityConverter). Cette erreur est critique.");
            }
            Visibility visibility = Visibility.Hidden;

            if((value as bool?).Value)
            {
                visibility = Visibility.Visible;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
