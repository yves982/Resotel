using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;

namespace ResotelApp.ViewModels
{
    class RoomChoiceViewModel
    {
        
        public DelegateCommand<INavigationService> OnLoadCommand { get; }
        public DelegateCommand<object> NextCommand { get; }

        public List<BedKindChoice> AvailableBedKinds { get; }

        public RoomChoiceViewModel()
        {
            OnLoadCommand = new DelegateCommand<INavigationService>(_load);
            NextCommand = new DelegateCommand<object>(_next);
            AvailableBedKinds = new List<BedKindChoice>();

        }

        private void _load(INavigationService obj)
        {
            throw new NotImplementedException();
        }

        private void _next(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
