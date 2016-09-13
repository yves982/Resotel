using ResotelApp.ViewModels.Events;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class PromptViewModel : INotifyPropertyChanged
    {
        private DelegateCommand<object> _okCommand;
        private DelegateCommand<object> _cancelCommand;
        private PropertyChangeSupport _pcs;
        private string _result;
        private string _message;
        private string _title;
        private bool _hasInput;

        public event PromptClosedEventHandler PromptClosed;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public bool? ShouldClose { get; set; }
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                _pcs.NotifyChange();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                _pcs.NotifyChange();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _pcs.NotifyChange();
            }
        }

        public bool HasInput
        {
            get { return _hasInput; }
            set
            {
                _hasInput = value;
                _pcs.NotifyChange();
            }
        }

        public DelegateCommand<object> OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new DelegateCommand<object>(_ok);
                }
                return _okCommand;
            }
        }

        public DelegateCommand<object> CancelCommand
        {
            get
            {
                if(_cancelCommand == null)
                {
                    _cancelCommand = new DelegateCommand<object>(_cancel);
                }
                return _cancelCommand;
            }
        }

        public PromptViewModel(string title, string message, bool hasInput=true)
        {
            _pcs = new PropertyChangeSupport(this);
            _title = title;
            _message = message;
            _hasInput = hasInput;
        }

        private void _ok(object ignore)
        {
            if(_result == null)
            {
                _result = "";
            }
            ShouldClose = true;
            _pcs.NotifyChange(nameof(ShouldClose));
            OnPromptClosed(new PromptClosedEventArgs(Result));
        }

        private void _cancel(object ignore)
        {
            Result = null;
            ShouldClose = true;
            _pcs.NotifyChange(nameof(ShouldClose));
            OnPromptClosed(new PromptClosedEventArgs(Result));
        }

        private void OnPromptClosed(PromptClosedEventArgs pcea)
        {
            PromptClosed?.Invoke(pcea);
        }
    }
}
