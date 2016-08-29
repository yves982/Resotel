using ResotelApp.ViewModels.Utils;
using ResotelApp.Views;
using ResotelApp.Views.Utils;
using System.Windows;
using System.Windows.Data;

namespace ResotelApp
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ViewDriver viewDriver = new ViewDriver();
            ViewDriverProvider.ViewDriver = viewDriver;

            CollectionViewProvider.Provider = CollectionViewSource.GetDefaultView;

            Window loginWindow = new LoginView();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.Show();
        }
    }
}
