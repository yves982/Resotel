using ResotelApp.Models;
using System.Windows.Data;
using System;
using System.Globalization;

namespace ResotelApp.Views.Converters
{
    [ValueConversion(typeof(RoomKind), typeof(string))]
    class RoomKindToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is RoomKind))
            {
                throw new ArgumentException("Seules les instances de Resotel.Models.RoomKind peuvent êtres converties", "value"); 
            }

            return $"/Resources/room_{((RoomKind)value).ToString()}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
