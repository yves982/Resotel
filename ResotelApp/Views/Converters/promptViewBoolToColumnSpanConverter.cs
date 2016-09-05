using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ResotelApp.Views.Converters
{
    /// <summary> Hacky converter used only by PromptView</summary>
    /// <remarks>The value comes from PromptViewModel.HasInput property
    /// Admittedly a PromptView without an input box is somewhat unnatural, 
    /// but I couldn't bother to make a MessageViewModel after a PromptViewModel
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(int))]
    class PromptViewBoolToColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool))
            {
                throw new ArgumentException("Seule les valeurs booléeenes peuvent être converties en entier (PromptViewBoolToColumnSpanConverter).", "value");
            }

            if(!(bool)value)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
