using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class ClientBookingsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private ICollectionView _clientBookingsView;
        private DelegateCommand<object> _selectBookingCommand;

        public ICollectionView ClientBookingsView
        {
            get { return _clientBookingsView; }
        }

        public ClientEntity Client
        {
            get { return _clientEntity; }
        }

        public string Title
        {
            get { return $"Réservations de : {_clientEntity.FirstName} {_clientEntity.LastName} - {_clientEntity.BirthDate:dd/MM/yyyy}"; }
        }

        public ICommand SelectBookingCommand
        {
            get { return _selectBookingCommand; }
        }

        public bool? ShouldClose { get; set; }

        public event EventHandler<BookingEntity> BookingSelected;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public ClientBookingsViewModel(ClientEntity clientEntity)
        {
            _pcs = new PropertyChangeSupport(this);
            _clientEntity = clientEntity;
            _clientBookingsView = CollectionViewProvider.Provider(clientEntity.Bookings);
            Booking booking = default(Booking);
            _clientBookingsView.SortDescriptions.Add(new SortDescription($"{nameof(booking.Dates)}.{nameof(booking.Dates.Start)}", ListSortDirection.Ascending));
            _clientBookingsView.CurrentChanged += _clientBookingsView_currentChanged;

            _selectBookingCommand = new DelegateCommand<object>(_selectBooking);
        }

        private void _clientBookingsView_currentChanged(object sender, EventArgs e)
        {
            bool nothingSelected = _clientBookingsView.CurrentPosition == -1;
            bool somethingSelected = !nothingSelected;
            bool canExecute = _selectBookingCommand.CanExecute(null);

            if(
                (somethingSelected && !canExecute) ||
                (nothingSelected && canExecute)
            )
            {
                _selectBookingCommand.ChangeCanExecute();
            }
        }

        private void _selectBooking(object obj)
        {
            if (_clientBookingsView.CurrentPosition != -1)
            {
                BookingEntity selectedBookingEntity = _clientBookingsView.CurrentItem as BookingEntity;
                BookingSelected?.Invoke(this, selectedBookingEntity);
                ShouldClose = true;
                _pcs.NotifyChange(nameof(ShouldClose));
            }
        }
    }
}
