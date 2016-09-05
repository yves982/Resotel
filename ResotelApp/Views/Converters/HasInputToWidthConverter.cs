using System;
using System.Globalization;
using System.Windows.Data;

namespace ResotelApp.Views.Converters
{
    /// <summary> Converter used by PromptView to handle input / inputless modes.</summary>
    /// <remarks>
    ///     This is a rather awkward hack, but I've used a switch property for an inputless promptView, so ...
    ///     the bottom line is, modal views aren't that mvvm-ish, although they're quite handy
    ///      (granted hidden panels still eat ram and are probably handled by the layout mechanism somehow)
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(double))]
    class HasInputToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = 120d;
            if(!(value is bool))
            {
                throw new InvalidOperationException("Seuls des booléens peuvent êtres convertis en largeur (HasInputToWidthConverter). Cette erreur est critique.");
            }
            bool hasInput = (bool)value;
            if(!hasInput)
            {
                width = 430d;
            }
            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
