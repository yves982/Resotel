using System;
using ResotelApp.ViewModels;
using System.Reflection;
using System.Windows;

namespace ResotelApp.Views
{
    class ViewDriver : IViewDriver
    {
        public void ShowView<T>(T viewModel) where T : class
        {
            string viewType = typeof(T).FullName.Replace("ViewModel","View");
            string assemblyName = Assembly.GetExecutingAssembly().CodeBase;
            object viewInstance = Activator.CreateInstanceFrom(assemblyName, viewType).Unwrap();

            if(viewInstance is Window)
            {
                ((Window)viewInstance).DataContext = viewModel;
                ((Window)viewInstance).ShowDialog();
            }
        }
    }
}
