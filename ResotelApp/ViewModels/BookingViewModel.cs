using ResotelApp.Models;
using ResotelApp.Repositories;
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
        private bool _parametersValidated;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;

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

        public ClientEntity Client
        {
            get { return _clientEntity; }
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

        public bool ParametersValidated
        {
            get { return _parametersValidated; }
            set
            {
                _parametersValidated = value;
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
                    _newClientCommand = new DelegateCommandAsync<BookingViewModel>(_newClient, _parametersValidated);
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
                    _searchClientCommand = new DelegateCommandAsync<BookingViewModel>(_searchClient, _parametersValidated);
                }
                return _searchClientCommand;
            }
        }

        public BookingViewModel(LinkedList<INavigableViewModel> navigation, Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _parameters = new BookingParametersViewModel(booking);
            _parameters.Defined += _parameters_defined;
            _parameters.PropertyChanged += _parametersChanged;
            _parametersValidated = false;
            _booking = booking;
            _clientEntity = new ClientEntity(booking.Client);
            _clientEntity.Bookings.Add(booking);
            _computeTitle(_clientEntity);
            _clientEntity.PropertyChanged += _clientChanged;
            _navigation = navigation;
            _navigation.AddLast(this);
        }

        ~BookingViewModel()
        {
            _parameters.Defined -= _parameters_defined;
            _parameters.PropertyChanged -= _parametersChanged;
            _clientEntity.PropertyChanged -= _clientChanged;
            if(Shutdown != null)
            {
                Shutdown(this, this);
            }
        }

        private async void _parameters_defined(object sender, BookingParametersViewModel bookingParametersVM)
        {
            ParametersValidated = true;
            _newClientCommand.ChangeCanExecute();
            _searchClientCommand.ChangeCanExecute();
            List<Option> availableOptions = await OptionRepository.GetAvailablesBetweenAsync(bookingParametersVM.DateRange);
            List<Room> availableRooms = await RoomRepository.GetAvailablesBetweenAsync(bookingParametersVM.DateRange);

            List<OptionChoiceEntity> availableOptionEntities = new List<OptionChoiceEntity>();
            List<RoomChoiceEntity> availableRoomChoiceEntities = new List<RoomChoiceEntity>();
            Dictionary<RoomKind, int> availableRoomKinds = new Dictionary<RoomKind, int>();
            foreach(Option opt in availableOptions)
            {
                OptionChoice optChoice = new OptionChoice {
                    Option = opt,
                    TakenDates = (DateRange)((ICloneable)bookingParametersVM.DateRange).Clone()
                };
                OptionChoiceEntity optionEntity = new OptionChoiceEntity(optChoice);
                availableOptionEntities.Add(optionEntity);
            }

            foreach(Room room in availableRooms)
            {
                if(!availableRoomKinds.ContainsKey(room.Kind))
                {
                    availableRoomKinds.Add(room.Kind, 1);
                    RoomChoiceEntity roomChoice = new RoomChoiceEntity(room.Kind, 0);
                    availableRoomChoiceEntities.Add(roomChoice);
                } else
                {
                    availableRoomKinds[room.Kind]++;
                }
            }

            foreach(RoomChoiceEntity roomChoice in availableRoomChoiceEntities)
            {
                roomChoice.MaxAvailable = availableRoomKinds[roomChoice.RoomKind];
            }

            Options = new OptionsViewModel(_booking, availableOptionEntities);
            RoomsChoices = new RoomsChoiceViewModel(availableRoomChoiceEntities);
            RoomsChoices.AvailableRoomsChoicesView.Filter = _mustShowRoomChoice;
        }

        private bool _mustShowRoomChoice(object item)
        {
            if(! (item is RoomChoiceEntity) )
            {
                throw new InvalidOperationException("On ne peut pas filtrer la liste des choix de chambre. Cette exception est critique");
            }
            RoomChoiceEntity roomChoice = (RoomChoiceEntity)item;
            return !roomChoice.RoomKind.Equals(RoomKind.DoubleWithBaby) || _parameters.BabiesCount > 0;
        }

        private void _parametersChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_newClientCommand.CanExecute(null))
            {
                ParametersValidated = false;
                _newClientCommand.ChangeCanExecute();
                _searchClientCommand.ChangeCanExecute();
            }
        }

        private void _computeTitle(ClientEntity clientEntity)
        {
            string clientDesc = null;
            if (clientEntity.FirstName != null || clientEntity.LastName != null)
            {
                clientDesc = string.Format("{0} {1}", clientEntity.FirstName, clientEntity.LastName);
            }
            _title = string.Format("Réservation: {0}{1:HH mm ss}", clientDesc, clientDesc == null ? (DateTime?)_booking.CreationDate : null);
        }

        private void _clientChanged(object sender, PropertyChangedEventArgs pcea)
        {
            if(pcea.PropertyName != "FirstName" && pcea.PropertyName != "LastName")
            {
                return;
            }
            _computeTitle(_clientEntity);
            _pcs.NotifyChange("Title");
        }

        private async Task _newClient(BookingViewModel bookingVM)
        {
            ClientViewModel clientVM = new ClientViewModel(_navigation, _clientEntity);
            await _roomsChoices.AssignRoomsCommand.ExecuteAsync(_booking);
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
