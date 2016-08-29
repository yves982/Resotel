using System;
using System.Globalization;
using System.Windows.Data;

namespace CustomControlsTest.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    class DoubleToDateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is double))
            {
                throw new ArgumentException("Cannot convert non double values.", "value");
            }
            string formattedDateTime = new DateTime((long)(double)value).ToString("dd/MM/yyyy", new CultureInfo("fr-FR"));
            return formattedDateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
