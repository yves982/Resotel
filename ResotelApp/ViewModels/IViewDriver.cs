using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    interface IViewDriver
    {
        void ShowView<T>(T viewModel) where T : class;
    }
}
