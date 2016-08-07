using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class BookingViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private BookingParametersViewModel _parameters;
        private OptionsViewModel _options;
        private RoomsChoiceViewModel _roomsChoices;
        private Booking _booking;
        private ClientEntity _clientEntity;
        private string _title;
        private LinkedList<INavigableViewModel> _navigation;

        private DelegateCommandAsync<BookingViewModel> _newClientCommand;
        private DelegateCommandAsync<BookingViewModel> _searchClientCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;

        public event EventHandler<INavigableViewModel> PreviousCalled;

        public BookingParametersViewModel Parameters
        {
            get { return _parameters; }

            set
            {
                _parameters = value;
                _pcs.NotifyChange();
            }
        }

        public OptionsViewModel Options
        {
            get { return _options; }

            set
            {
                _options = value;
                _pcs.NotifyChange();
            }
        }

        public RoomsChoiceViewModel RoomsChoices
        {
            get { return _roomsChoices; }

            set
            {
                _roomsChoices = value;
                _pcs.NotifyChange();
            }
        }

        public Booking Booking
        {
            get { return _booking; }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _pcs.NotifyChange();
            }
        }

        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }

        public ICommand NewClientCommand
        {
            get
            {
                if(_newClientCommand == null)
                {
                    _newClientCommand = new DelegateCommandAsync<BookingViewModel>(_newClient);
                }
                return _newClientCommand;
            }
        }

        public ICommand SearchClientCommand
        {
            get
            {
                if(_searchClientCommand == null)
                {
                    _searchClientCommand = new DelegateCommandAsync<BookingViewModel>(_searchClient);
                }
                return _searchClientCommand;
            }
        }

        public BookingViewModel(LinkedList<INavigableViewModel> navigation, Booking booking, List<OptionEntity> availableOptions, List<RoomChoiceEntity> availableRoomChoices)
        {
            _pcs = new PropertyChangeSupport(this);
            _parameters = new BookingParametersViewModel(booking);
            _options = new OptionsViewModel(booking, availableOptions);
            _roomsChoices = new RoomsChoiceViewModel(availableRoomChoices);
            _booking = booking;
            _clientEntity = new ClientEntity(booking.Client);
            _computeTitle(booking.Client);
            _clientEntity.PropertyChanged += _clientChanged;
            _navigation = navigation;
            _navigation.AddLast(this);
        }

        private void _computeTitle(Client client)
        {
            string clientDesc = null;
            if (client.FirstName != null || client.LastName != null)
            {
                clientDesc = string.Format("{0} {1}", client.FirstName, client.LastName);
            }
            _title = string.Format("Réservation: {0}{1:HH mm ss}", clientDesc, clientDesc == null ? (DateTime?)_booking.CreationDate : null);
        }

        private void _clientChanged(object sender, PropertyChangedEventArgs pcea)
        {
            if(pcea.PropertyName != "FirstName" && pcea.PropertyName != "LastName")
            {
                return;
            }
            _computeTitle(_booking.Client);
            _pcs.NotifyChange("Title");
        }

        private async Task _newClient(BookingViewModel bookingVM)
        {
            Client newClient = new Client();
            ClientViewModel clientVM = new ClientViewModel(_navigation, newClient);
            await _roomsChoices.AssignRoomsCommand.ExecuteAsync(bookingVM.Booking);
            if(NextCalled != null)
            {
                NextCalled(this, bookingVM);
            }
        }

        private async Task _searchClient(BookingViewModel bookingVM)
        {
            throw new NotImplementedException();
        }
    }
}
