using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ResotelApp.ViewModels.Utils
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class RequiredFieldsContentValueConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = parameter.ToString();
            List<string> requiredStrings = value as List<string>;
            if (requiredStrings.Contains(parameter.ToString()))
            {
                res = "(*) " +res;
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
