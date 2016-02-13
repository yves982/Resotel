using ResotelApp.ViewModels.Utils;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using ResotelApp.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class MainWindowModel : NotifyPropertyChangedSupport
    {
        private List<string> _userControls;
        private BiDirectionalIterator<string> _userControlsIterator;
        

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
            _userControls = new List<string> {
                "ClientInfos", "OptionsChoices"
            };
            _userControlsIterator = new BiDirectionalIterator<string>(_userControls);
            HasNext = _userControlsIterator.HasNext;
            HasPrev = _userControlsIterator.HasPrev;
            CurrentControlName = _userControlsIterator.Current;
        }


        private void _aboutClicked(object sender)
        {
            MessageBox.Show("AIFONE est une application de gestion d'hotel de démonstration", "AIFONE**", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void _nextClicked(object sender)
        {
            _userControlsIterator.MoveNext();
            CurrentControlName = _userControlsIterator.Current;
            HasNext = _userControlsIterator.HasNext;
            HasPrev = _userControlsIterator.HasPrev;
        }

        private void _prevClicked(object sender)
        {
            _userControlsIterator.MovePrev();
            CurrentControlName = _userControlsIterator.Current;
            HasNext = _userControlsIterator.HasNext;
            HasPrev = _userControlsIterator.HasPrev;
        }

        
    }
}
