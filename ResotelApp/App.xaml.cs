using ResotelApp.Models;
using ResotelApp.Models.Context;
using ResotelApp.ViewModels;
using ResotelApp.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace ResotelApp
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Window mainWindow = new MainWindow();
            MainWindowModel mainModel = new MainWindowModel();
            mainModel.CreateCommands();
            mainWindow.DataContext = mainModel;
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
        }
    }
}
