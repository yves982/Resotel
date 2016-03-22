using ResotelApp.ViewModels.Utils;

namespace ResotelApp.ViewModels
{
    class PlanningViewModel
    {
        private INavigationService _nav;
        public INavigationService Nav
        {
            get { return _nav; } 
        }

        public DelegateCommand<INavigationService> OnLoadCommand { get; }

        public PlanningViewModel()
        {
            OnLoadCommand = new DelegateCommand<INavigationService>(_onLoad);
        }

        private void _onLoad(INavigationService navService)
        {
            _nav = navService;
        }
    }
}
