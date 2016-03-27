using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class BookingParametersViewModel
    {
        private INavigationService _nav;
        public INavigationService Nav
        {
            get { return _nav; }
        }

        public DelegateCommand<INavigationService> OnLoadCommand { get; }

        public BookingParametersViewModel()
        {
            OnLoadCommand = new DelegateCommand<INavigationService>(_onLoad);
        }

        private void _onLoad(INavigationService navService)
        {
            _nav = navService;
        }
    }
}
