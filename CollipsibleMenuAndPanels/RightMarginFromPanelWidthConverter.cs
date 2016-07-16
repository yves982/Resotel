using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CollipsibleMenuAndPanels
{
    [ValueConversion(sourceType: typeof(Double), targetType:typeof(Thickness))]
    class RightMarginFromPanelWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(! (value is Double) )
            {
                throw new ArgumentException("Invalid type supplied, expected Double got: " + value.GetType().Name, "value");
            }
            double rightMargin = ((double)value);
            return new Thickness(rightMargin, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
