using ResotelApp.ViewModels.Utils;
using System.Windows;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResotelApp.ViewModels
{
    class MainWindowModel : NotifyPropertyChangedSupport
    {
        private List<string> _userControls;
        

        public ICommand AboutCommand { get; private set; }
        public ICommand NextCommand { get; private set; }
        public ICommand PrevCommand { get; private set; }

        private string _currentControlName;
        private bool _hasNext;
        private bool _hasPrev;

        public string CurrentControlName
        {
            get { return _currentControlName; }
            set
            {
                SetField(ref _currentControlName, value);
                HasNext = _currentControlName != "LastPage";
                HasPrev = _currentControlName != "StartPage";
            }
        }



        public bool HasPrev
        {
            get { return _hasPrev; }
            private set
            {
                _hasPrev = value;
                SetField(ref _hasPrev, value);
            }
        }

        public bool HasNext
        {
            get { return _hasNext; }
            private set
            {
                _hasNext = value;
                SetField(ref _hasNext, value);
            }
        }

        public void CreateCommands()
        {
            AboutCommand = new DelegateCommand(_aboutClicked, null);
            NextCommand = new DelegateCommand(_nextClicked, null);
            PrevCommand = new DelegateCommand(_prevClicked, null);
        }

        public MainWindowModel()
        {
            _userControls = new List<string> { "StartPage" };
            _currentControlName = "StartPage";
            _hasNext = true;
            _hasPrev = false;
        }


        private void _aboutClicked(object obj)
        {
            MessageBox.Show("AIFONE est une application de gestion d'hotel de démonstration", "AIFONE**", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void _nextClicked(object obj)
        {
            CurrentControlName = "Tutu";
            //throw new NotImplementedException();
        }

        private void _prevClicked(object obj)
        {
            CurrentControlName = "StartPage";
        }

        
    }
}
