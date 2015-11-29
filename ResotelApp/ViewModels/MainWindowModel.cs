using ResotelApp.ViewModels.Utils;
using System.Windows.Input;
using System;
using System.Windows;

namespace ResotelApp.ViewModels
{
    class MainWindowModel
    {
        public ICommand AboutCommand { get; private set; }

        public void CreateCommands()
        {
            AboutCommand = new DelegateCommand(_aboutClicked, (obj) => true);
        }

        private void _aboutClicked(object obj)
        {
            MessageBox.Show("AIFONE est une application de gestion d'hotel de démonstration", "AIFONE**", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
