using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResotelApp.Views.Converters
{
    [ValueConversion(typeof(Enum), typeof(bool), ParameterType = typeof(string))]
    class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Enum))
            {
                throw new InvalidOperationException("Seuls les énumération peuvent êtres converties (EnumToBoolConverter). Cette erreur est critique.");
            }

            string paramStr = parameter as string;
            if (paramStr == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), paramStr);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("Seuls les booléen peuvent êtres convertis (EnumToBoolConverter:ConvertBack). Cette erreur est critique.");
            }

            string paramStr = parameter as string;
            if (paramStr == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(targetType, paramStr) == false || !((bool)value))
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(targetType, paramStr);

            return parameterValue;
        }
    }
}
