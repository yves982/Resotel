using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResotelApp.ViewModels.Utils
{
    public interface IAsyncCommand<T> : ICommand where T : class
    {
        Task ExecuteAsync(object parameter);
    }
}
