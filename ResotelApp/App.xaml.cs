using ResotelApp.ViewModels.Utils;
using ResotelApp.Views;
using System.Windows;

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

            Window mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
        }
    }
}
