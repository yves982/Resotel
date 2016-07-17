using ResotelApp.ViewModels.Events;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class PromptViewModel : IViewModel, INotifyPropertyChanged
    {
        private DelegateCommand<object> _okCommand;
        private DelegateCommand<object> _cancelCommand;

        public event PromptClosedEventHandler PromptClosed;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool? ShouldClose { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }

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

        public PromptViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        private void _ok(object ignore)
        {
            ShouldClose = true;
            PropertyChangeSupport.NotifyChange(this, PropertyChanged, nameof(ShouldClose));
            OnPromptClosed(new PromptClosedEventArgs(Result));
        }

        private void _cancel(object ignore)
        {
            Result = null;
            ShouldClose = true;
            PropertyChangeSupport.NotifyChange(this, PropertyChanged, nameof(ShouldClose));
            OnPromptClosed(new PromptClosedEventArgs(Result));
        }

        private void OnPromptClosed(PromptClosedEventArgs pcea)
        {
            if(PromptClosed != null)
            {
                PromptClosed(pcea);
            }
        }
    }
}
