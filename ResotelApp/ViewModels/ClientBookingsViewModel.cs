using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using ResotelApp.DAL;

namespace ResotelApp.ViewModels
{
    class ClientBookingsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private ICollectionView _clientBookingsView;
        private ICollectionViewSource _clientBookingsSource;
        private DelegateCommand<object> _selectBookingCommand;
        private DelegateCommandAsync<object> _cancelBookingCommand;

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

        public ICommand CancelBookingCommand
        {
            get { return _cancelBookingCommand; }
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
            _clientBookingsSource = CollectionViewProvider.Provider(clientEntity.Bookings);
            _clientBookingsView = _clientBookingsSource.View;
            Booking booking = default(Booking);
            _clientBookingsView.SortDescriptions.Add(new SortDescription($"{nameof(booking.Dates)}.{nameof(booking.Dates.Start)}", ListSortDirection.Ascending));
            _clientBookingsView.CurrentChanged += _clientBookingsView_currentChanged;

            _selectBookingCommand = new DelegateCommand<object>(_selectBooking);
            _cancelBookingCommand = new DelegateCommandAsync<object>(_cancelBooking);

            _clientBookingsView.Filter = _mustShowBooking;
        }

        ~ClientBookingsViewModel()
        {
            if(_clientBookingsView != null)
            {
                _clientBookingsView.CurrentChanged -= _clientBookingsView_currentChanged;
            }
        }

        private bool _mustShowBooking(object bookingEntity)
        {
            BookingEntity bookingE = bookingEntity as BookingEntity;
            return bookingE.State == BookingState.Validated || bookingE.State == BookingState.Paid;
        }

        private async Task _cancelBooking(object arg)
        {
            int bookingPosition = _clientBookingsView.CurrentPosition;
            if(bookingPosition != -1)
            {
                BookingEntity selectedBookingEntity = _clientBookingsView.CurrentItem as BookingEntity;

                selectedBookingEntity.TerminatedDate = DateTime.Now.Date;
                await BookingRepository.Save(selectedBookingEntity.Booking);
                bool refunded = selectedBookingEntity.State == BookingState.FullyCancelled;
                if(refunded)
                {
                    PromptViewModel successPromptVM = new PromptViewModel("Succés", "La réservation a été annulée.", false);
                    ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(successPromptVM);
                } else
                {
                    string complement = selectedBookingEntity.Payment.Ammount > 0d ? "ne sera pas remboursée" : "reste due";
                    PromptViewModel successPromptVM = new PromptViewModel("Succés", $"La réservation a été annulée, mais {complement}.", false);
                    ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(successPromptVM);
                }
            }
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
                _cancelBookingCommand.ChangeCanExecute();
            }
        }

        private void _selectBooking(object ignore)
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
