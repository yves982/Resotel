using ResotelApp.ViewModels.Utils;
using ResotelApp.Views;
using ResotelApp.Views.Utils;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ResotelApp
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ICollectionViewSource _provider(IEnumerable source)
        {
            CollectionViewSource cvs = new CollectionViewSource
            {
                Source = source,
                IsLiveFilteringRequested = true
            };
            ICollectionViewSource iCvs = new CollectionViewSourceImpl(cvs);
            return iCvs;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ViewDriver viewDriver = new ViewDriver();
            ViewDriverProvider.ViewDriver = viewDriver;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //CollectionViewProvider.Provider = CollectionViewSource.GetDefaultView;

            CollectionViewProvider.Provider = _provider;

            Window loginWindow = new LoginView();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.Show();
        }
    }
}
