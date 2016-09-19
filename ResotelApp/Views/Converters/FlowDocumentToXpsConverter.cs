using ResotelApp.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace ResotelApp.Views.Converters
{
    class FlowDocumentToXpsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(values.Length > 1 && values[0] is FlowDocument))
            {
                throw new InvalidOperationException("Seule les valeurs de type FlowDocument peuvent êtres converties (FlowDocumentToXpsConverter). Cette erreur est critique.");
            }
            
            if(!(values.Length > 1 && values[1] is SumUpViewModel) && !(values[1] == null))
            {
                throw new InvalidOperationException("Une valeur (position 2) de type SumUpViewModel doit être fournie (FlowDocumentToXpsConverter. Cette erreur est critique");
            }

            if (values[1] != null)
            {
                FlowDocument flowDoc = ((FlowDocument)values[0]);
                flowDoc.DataContext = values[1];

                using (FileStream fs = File.Open("flowDocument.xps", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (Package package = Package.Open(fs, FileMode.Create, FileAccess.ReadWrite))
                    {
                        XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum);
                        XpsSerializationManager xpsSerializationManager = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                        DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDoc).DocumentPaginator;
                        xpsSerializationManager.SaveAsXaml(paginator);
                        xpsSerializationManager.Commit();

                        return xpsDoc;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
