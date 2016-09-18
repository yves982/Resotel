using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ResotelApp.Views.Converters
{
    [ValueConversion(typeof(bool), typeof(Color))]
    class ActiveFixedDatesToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Colors.Transparent;
            if(!(value is bool))
            {
                throw new InvalidOperationException("Seule les valeurs booléennes peuvent êtres converties (ActiveFixedDatesToSolidColorBrushConverter). Cette erreur est critique");
            }

            bool hasActiveFixedDates = (bool)value;

            if (hasActiveFixedDates)
            {
                color = Colors.Black;
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
