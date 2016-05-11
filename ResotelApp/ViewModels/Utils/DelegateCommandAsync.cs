using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResotelApp.ViewModels.Utils
{
    class DelegateCommandAsync<TParam> : IAsyncCommand<TParam> where TParam:class
    {
        private readonly Predicate<TParam> _canExecute;
        private readonly Func<TParam, Task> _executeAsync;

        public event EventHandler CanExecuteChanged;

        public DelegateCommandAsync(Func<TParam,Task> executeAsync)
                       : this(executeAsync, null)
        {
        }

        public DelegateCommandAsync(Func<TParam, Task> executeAsync,
                       Predicate<TParam> canExecute)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
            if (CanExecuteChanged != null)
            {
                EventArgs e = canExecute as Object as EventArgs;
                CanExecuteChanged(this, e);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((TParam)parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            await _executeAsync((TParam)parameter);
        }
    }
}
