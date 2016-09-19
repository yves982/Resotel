using ResotelApp.Utils;
using ResotelApp.ViewModels.Utils;
using System;
using System.Reflection;
using System.Windows;

namespace ResotelApp.Views.Utils
{

    class ViewDriver : IViewDriver
    {

        public void ShowView<T>(T viewModel) where T : class
        {
            try
            {
                object viewInstance = _getView<T>();

                if (viewInstance is Window)
                {
                    ((Window)viewInstance).WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ((Window)viewInstance).DataContext = viewModel;
                    ((Window)viewInstance).Topmost = true;
                    ((Window)viewInstance).Activate();
                    ((Window)viewInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // central log #1
                Logger.Log(ex);
            }
        }

        public void CloseAndShowNewMainWindow<T>(T viewModel) where T : class
        {
            try
            {
                object viewInstance = _getView<T>();

                if (viewInstance is Window)
                {
                    Window oldWin = Application.Current.MainWindow;
                    Application.Current.MainWindow = (Window)viewInstance;
                    oldWin.Close();
                    ((Window)viewInstance).WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ((Window)viewInstance).DataContext = viewModel;
                    ((Window)viewInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // central log #2
                Logger.Log(ex);
            }
        }

        private static object _getView<T>() where T : class
        {
            string viewType = typeof(T).FullName.Replace("ViewModel", "View");
            string assemblyName = Assembly.GetExecutingAssembly().CodeBase;
            object viewInstance = Activator.CreateInstanceFrom(assemblyName, viewType).Unwrap();
            return viewInstance;
        }
    }
}
