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
        public DelegateCommand<object> NextCommand { get; }
        public string NextTitle
        {
            get { return TranslationHandler.GetString("NextTitle"); }
        }

        public BookingParametersViewModel()
        {
            OnLoadCommand = new DelegateCommand<INavigationService>(_onLoad);
            NextCommand = new DelegateCommand<object>(_next);
        }

        private void _next(object empty)
        {
           _nav.Navigate(new Uri("../Views/RoomsView.xaml", UriKind.Relative));
        }

        private void _onLoad(INavigationService navService)
        {
            _nav = navService;
        }
    }
}
