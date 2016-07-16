using ResotelApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ResotelApp.ViewModels
{
    class MainWindowViewModel : IViewModel
    {
        private ICollectionView _currentBookingsView;
        private ObservableCollection<Booking> _currentBookings;

        public ICollectionView CurrentBookings
        {
            get
            {
                if(_currentBookingsView != null)
                {
                    _currentBookingsView = CollectionViewSource.GetDefaultView(_currentBookings);
                }
                return _currentBookingsView;
            }
        }

        public MainWindowViewModel()
        {
            _currentBookings = new ObservableCollection<Booking>();
        }
    }

}
