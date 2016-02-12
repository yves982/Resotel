using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ResotelApp.ViewModels.Utils
{
    [ValueConversion(typeof(List<string>), typeof(FontWeight))]
    public class RequiredFieldsFontWeightValueConverter : MarkupExtension,  IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           FontWeight res = FontWeights.Normal;
            List<string> requiredStrings = value as List<string>;
            if(requiredStrings.Contains(parameter.ToString()))
            {
                res = FontWeights.Bold;
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
