using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResotelApp.ViewModels.Utils
{
    class DelegateCommandAsync<T> : ICommand where T : class
    {
        private Func<T, Task> _executeAsync;
        private bool _canExecute;


        public event EventHandler CanExecuteChanged;


        public DelegateCommandAsync(Func<T,Task>executeAsync, bool canExecute = true)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public void ChangeCanExecute()
        {
            _canExecute = !_canExecute;
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        public async Task ExecuteAsync(T parameter)
        {
            await _executeAsync(parameter);
        }
    }
}
