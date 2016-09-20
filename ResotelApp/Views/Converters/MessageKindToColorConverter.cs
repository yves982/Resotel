using ResotelApp.ViewModels.Entities;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ResotelApp.Views.Converters
{
    [ValueConversion(typeof(MessageKind), typeof(SolidColorBrush))]
    class MessageKindToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is MessageKind))
            {
                throw new InvalidOperationException("On ne peut convertir que des MessageKind (MessageKindToColorConverter). Cette erreur est critique.");
            }
            MessageKind msgKind = (MessageKind)value;
            SolidColorBrush messageBrush = default(SolidColorBrush);
            switch(msgKind)
            {
                case MessageKind.Error:
                    messageBrush = Brushes.Red;
                    break;
                case MessageKind.Standard:
                    messageBrush = Brushes.Black;
                    break;
            }
            return messageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
