using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CustomControlsTest.Converters
{
    [ValueConversion(typeof(DateTime), typeof(double))]
    class DateTimeDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is DateTime))
            {
                throw new ArgumentException("Cannot convert non DateTime values", "value");
            }
            return ((DateTime)value).Ticks;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is double))
            {
                throw new ArgumentException("Cannot convert back non double values", "value");
            }
            return new DateTime((long)(double)value);
        }
    }
}
