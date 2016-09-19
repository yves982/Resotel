using ResotelApp.DAL;
using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Events;
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
        private RoomsChoiceViewModel _roomChoices;
        private Booking _booking;
        private BookingEntity _bookingEntity;
        private ClientEntity _clientEntity;
        private string _title;
        private LinkedList<INavigableViewModel> _navigation;

        private DelegateCommandAsync<BookingViewModel> _newClientCommand;
        private DelegateCommandAsync<BookingViewModel> _searchClientCommand;
        private DelegateCommandAsync<BookingViewModel> _validateBookingCommand;

        private bool _parametersValidated;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;
        public event EventHandler<string> MessageReceived;

        public bool IsSaved
        {
            get { return _booking.Id > 0; }
        }

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
                if (_options != null)
                {
                    _options.OptionChanged -= _optionsChanged;
                }

                _options = value;

                if (_options != null)
                {
                    _options.OptionChanged += _optionsChanged;
                }
                _pcs.NotifyChange();
            }
        }

        public RoomsChoiceViewModel RoomChoices
        {
            get { return _roomChoices; }

            set
            {
                _roomChoices = value;
                _pcs.NotifyChange();
            }
        }

        public Booking Booking
        {
            get { return _booking; }
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

        public IEnumerable<BookedRoomEntity> AssignedRooms
        {
            get
            {
                List<BookedRoomEntity> bookedRooms = _booking.Rooms.ConvertAll(
                    room => new BookedRoomEntity(_booking, room)
                );
                return bookedRooms;
            }
        }

        public bool IsEditing
        {
            get { return _booking.Id > 0; }
        }

        public bool IsNewBooking
        {
            get { return _booking.Id == 0; }
        }

        public ICommand NewClientCommand
        {
            get { return _newClientCommand; }
        }

        public ICommand SearchClientCommand
        {
            get { return _searchClientCommand; }
        }

        public ICommand ValidateBookingCommand
        {
            get { return _validateBookingCommand; }
        }

        public static async Task<BookingViewModel> LoadAsync(LinkedList<INavigableViewModel> navigation, Booking booking, LinkedListNode<INavigableViewModel> prevNode)
        {
            BookingViewModel bookingVM = new BookingViewModel(navigation, booking, prevNode);
            await bookingVM._validateParameters();

            return bookingVM;
        }

        public BookingViewModel(LinkedList<INavigableViewModel> navigation, Booking booking, LinkedListNode<INavigableViewModel> prevNode = null)
        {
            _pcs = new PropertyChangeSupport(this);
            _navigation = navigation;
            _parameters = new BookingParametersViewModel(booking);
            _parameters.Defined += _parameters_defined;
            _parameters.PropertyChanged += _parametersChanged;
            _parametersValidated = false;
            _booking = booking;
            _clientEntity = new ClientEntity(_booking.Client);
            _bookingEntity = new BookingEntity(_booking);
            _clientEntity.Bookings.Add(_bookingEntity);
            _computeTitle(_clientEntity);
            _clientEntity.PropertyChanged += _clientChanged;

            _searchClientCommand = new DelegateCommandAsync<BookingViewModel>(_searchClient, false);
            _newClientCommand = new DelegateCommandAsync<BookingViewModel>(_newClient, false);
            _validateBookingCommand = new DelegateCommandAsync<BookingViewModel>(_validateBooking, false);

            if (prevNode == null)
            {
                _navigation.AddLast(this);
            }
            else
            {
                _navigation.AddAfter(prevNode, this);
            }
        }

        ~BookingViewModel()
        {
            if (_parameters != null)
            {
                _parameters.Defined -= _parameters_defined;
                _parameters.PropertyChanged -= _parametersChanged;
            }

            if (_clientEntity != null)
            {
                _clientEntity.PropertyChanged -= _clientChanged;
            }

            if (_options != null)
            {
                _options.OptionChanged -= _optionsChanged;
            }

            Shutdown?.Invoke(null, this);
        }

        public async Task Save()
        {
            await BookingRepository.Save(_booking);
        }

        public async Task AssignRooms()
        {
            await _roomChoices.AssignRoomsCommand.ExecuteAsync(_booking);
            _checkRoomCapacityWithBabies(this);
        }

        private void _unlockSearchClientAndNewClientIfNeeded()
        {
            bool canSearch = _searchClientCommand.CanExecute(null);
            bool canNew = _searchClientCommand.CanExecute(null);
            bool validatesBookingDates = ((IDataErrorInfo)_booking.Dates).Error == null;
            bool validatesOptions = true;
            foreach(OptionChoiceEntity optChoiceEntity in _options.AvailableOptionChoiceEntitiesView)
            {
                if(((IDataErrorInfo)optChoiceEntity).Error != null && optChoiceEntity.Taken)
                {
                    validatesOptions = false;
                    break;
                }
            }

            if(
                (!canSearch && validatesOptions && validatesBookingDates)
                || (canSearch && (!validatesBookingDates || !validatesOptions))
                )
            {
                _searchClientCommand.ChangeCanExecute();
                _newClientCommand.ChangeCanExecute();
                _validateBookingCommand.ChangeCanExecute();
            }
        }

        private async Task _validateParameters()
        {
            _parametersValidated = true;
            bool newClientCanExecute = _newClientCommand.CanExecute(null);
            bool searchClientCanExecute = _searchClientCommand.CanExecute(null);
            bool validateBookingCanExecute = _validateBookingCommand.CanExecute(null);

            _parameters.ChangeValidateCanExecute();

            if (!newClientCanExecute)
            {
                _newClientCommand.ChangeCanExecute();
            }

            if (!searchClientCanExecute)
            {
                _searchClientCommand.ChangeCanExecute();
            }

            if(!validateBookingCanExecute)
            {
                _validateBookingCommand.ChangeCanExecute();
            }

            DateRange dateRange = new Models.DateRange
            {
                Start = _parameters.Start,
                End = _parameters.End
            };
            Options = await OptionsViewModel.CreateAsync(_booking, dateRange);
            RoomChoices = await RoomsChoiceViewModel.CreateAsync(dateRange);
            RoomChoices.AvailableRoomChoiceEntitiesView.Filter = _mustShowRoomChoice;

            foreach (OptionChoiceEntity optChoiceEntity in Options.AvailableOptionChoiceEntitiesView)
            {
                foreach (OptionChoice optChoice in _booking.OptionChoices)
                {
                    if (optChoiceEntity.OptionChoice.Option.Id == optChoice.Option.Id)
                    {
                        optChoiceEntity.Taken = true;
                        optChoiceEntity.TakenStart = optChoice.TakenDates.Start;
                        optChoiceEntity.TakenEnd = optChoice.TakenDates.End;
                        optChoiceEntity.PeopleCount = optChoice.PeopleCount;
                        break;
                    }
                }
            }

            Dictionary<RoomKind, int> takenRoomKind = new Dictionary<RoomKind, int>();

            foreach (Room room in _booking.Rooms)
            {
                if (!takenRoomKind.ContainsKey(room.Kind))
                {
                    takenRoomKind.Add(room.Kind, 0);
                }
                takenRoomKind[room.Kind]++;
            }

            foreach (RoomChoiceEntity roomChoiceEntity in RoomChoices.AvailableRoomChoiceEntitiesView)
            {
                if (takenRoomKind.ContainsKey(roomChoiceEntity.RoomKind))
                {
                    roomChoiceEntity.Count = takenRoomKind[roomChoiceEntity.RoomKind];
                }
            }
        }

        private async void _parameters_defined(object sender, BookingParametersViewModel bookingParametersVM)
        {
            ParametersValidated = true;
            DateRange dateRange = new DateRange
            {
                Start = _parameters.Start,
                End = _parameters.End
            };

            Options = await OptionsViewModel.CreateAsync(_booking, dateRange);
            RoomChoices = await RoomsChoiceViewModel.CreateAsync(dateRange);
            RoomChoices.AvailableRoomChoiceEntitiesView.Filter = _mustShowRoomChoice;

            _newClientCommand.ChangeCanExecute();
            _searchClientCommand.ChangeCanExecute();
            _validateBookingCommand.ChangeCanExecute();
        }

        private bool _mustShowRoomChoice(object item)
        {
            if (!(item is RoomChoiceEntity))
            {
                throw new InvalidOperationException("On ne peut pas filtrer la liste des choix de chambre. Cette exception est critique");
            }
            RoomChoiceEntity roomChoiceEntity = (RoomChoiceEntity)item;

            bool mustShowRoomChoice = !roomChoiceEntity.RoomKind.Equals(RoomKind.DoubleWithBaby) || _parameters.BabiesCount > 0;

            return mustShowRoomChoice;
        }

        private void _parametersChanged(object sender, PropertyChangedEventArgs e)
        {


            _booking.AdultsCount = _parameters.AdultsCount;
            _booking.BabiesCount = _parameters.BabiesCount;
            _booking.Dates.Start = _parameters.Start;
            _booking.Dates.End = _parameters.End;

            ParametersValidated = false;

            if (_newClientCommand.CanExecute(null))
            {
                _newClientCommand.ChangeCanExecute();
                _searchClientCommand.ChangeCanExecute();
                _validateBookingCommand.ChangeCanExecute();
                MessageReceived?.Invoke(null, null);
            }

            if (!_parameters.ValidateCommand.CanExecute(null))
            {
                _parameters.ChangeValidateCanExecute();
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
            if (pcea.PropertyName != "FirstName" && pcea.PropertyName != "LastName")
            {
                return;
            }
            _computeTitle(_clientEntity);
            _pcs.NotifyChange("Title");
        }

        private void _optionsChanged(object sender, OptionChoiceEntityChange optChoiceEntityChange)
        {
            OptionChoiceEntity optChoiceEntity = optChoiceEntityChange.OptionChoiceEntity;
            int optChoiceIndex = _booking.OptionChoices.FindIndex(optC => optC.Option.Id == optChoiceEntity.OptionChoice.Option.Id);

            if (optChoiceEntityChange.Kind == OptionChangeKind.Taken && optChoiceEntity.Taken)
            {
                DateRange dateRange = (DateRange)((ICloneable)_booking.Dates).Clone();
                optChoiceEntity.TakenStart = dateRange.Start;
                optChoiceEntity.TakenEnd = dateRange.End;

                if(optChoiceEntity.OptionChoice.Option.Id == 8)
                {
                    optChoiceEntity.TakenStart = dateRange.Start.Date.AddDays(1.0d);
                }
            }

            if (optChoiceEntity.Taken && optChoiceEntityChange.Kind == OptionChangeKind.Taken)
            {
                if (optChoiceIndex != -1)
                {
                    _booking.OptionChoices.RemoveAt(optChoiceIndex);
                }
                _booking.OptionChoices.Add(optChoiceEntity.OptionChoice);
            }


            else if (!optChoiceEntity.Taken && optChoiceEntityChange.Kind == OptionChangeKind.Taken && optChoiceIndex != -1)
            {
                _booking.OptionChoices.Remove(optChoiceEntity.OptionChoice);
            }

            RoomChoices.Update(optChoiceEntity);

            _unlockSearchClientAndNewClientIfNeeded();

            _checkCapacityWithBabies(optChoiceEntity);
        }

        private void _checkCapacityWithBabies(OptionChoiceEntity optChoiceEntity)
        {
            bool previouslyHadBabyRoom = hasAvailableBabyRoom();

            bool hasBabyRoom = hasAvailableBabyRoom();

            if (_parameters.BabiesCount > 0 && optChoiceEntity.Taken && previouslyHadBabyRoom)
            {

                if (!hasBabyRoom)
                {
                    MessageReceived?.Invoke(null, $"#Aucune chambre avec lit bébé ne possède l'option {optChoiceEntity.Description}!");
                }
                else
                {
                    MessageReceived?.Invoke(null, null);
                }
            }

            else if (_parameters.BabiesCount > 0 && hasBabyRoom && !previouslyHadBabyRoom)
            {
                MessageReceived?.Invoke(null, null);
            }
        }

        private bool hasAvailableBabyRoom()
        {
            bool babyRoomFound = false;
            foreach (RoomChoiceEntity roomChoiceEntity in RoomChoices.AvailableRoomChoiceEntitiesView)
            {
                if (roomChoiceEntity.RoomKind.Equals(RoomKind.DoubleWithBaby))
                {
                    babyRoomFound = true;
                    break;
                }
            }

            return babyRoomFound;
        }

        private async Task _newClient(BookingViewModel bookingVM)
        {
            LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(bookingVM);
            ClientViewModel clientVM = new ClientViewModel(_navigation, _clientEntity, prevNode);
            await _roomChoices.AssignRoomsCommand.ExecuteAsync(_booking);

            bool success = _checkRoomCapacityWithBabies(bookingVM);
            if (success)
            {
                NextCalled?.Invoke(null, bookingVM);
            }
        }

        private bool _checkRoomCapacityWithBabies(BookingViewModel bookingVM)
        {
            bool success = false;
            if (_clientEntity.Bookings != null && _clientEntity.Bookings.Count > 0)
            {
                int babiesRooms, totalAdultCapacity;
                _countBabiesAndAdultsCapacity(out babiesRooms, out totalAdultCapacity);

                if (babiesRooms < _booking.BabiesCount || totalAdultCapacity < _booking.AdultsCount)
                {
                    PromptViewModel insufficientRoomsPromptVM = new PromptViewModel("Erreur",
                        $"Il n'y a pas assez de chambre pour accueillir {_booking.BabiesCount} bébés et {_booking.AdultsCount} adultes.",
                        false
                    );
                    ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(insufficientRoomsPromptVM);
                }
                else
                {
                    success = true;
                }
            }
            return success;
        }

        private void _countBabiesAndAdultsCapacity(out int babiesRooms, out int totalAdultCapacity)
        {
            babiesRooms = 0;
            totalAdultCapacity = 0;
            foreach (Room room in _booking.Rooms)
            {
                if (room.Kind.Equals(RoomKind.DoubleWithBaby))
                {
                    babiesRooms++;
                }

                totalAdultCapacity += room.Capacity;
            }
        }

        private async Task _searchClient(BookingViewModel bookingVM)
        {
            List<Client> clients = await ClientRepository.GetAllClients();
            List<ClientEntity> clientEntities = new List<ClientEntity>(clients.Count);
            foreach (Client client in clients)
            {
                ClientEntity clientEntity = new ClientEntity(client);
                clientEntities.Add(clientEntity);
            }
            SearchClientsViewModel searchClientVM = new SearchClientsViewModel(clientEntities);
            searchClientVM.ClientSelected += _searchClient_clientSelected;
            ViewDriverProvider.ViewDriver.ShowView<SearchClientsViewModel>(searchClientVM);
        }

        private async void _searchClient_clientSelected(object sender, ClientEntity selectedClientEntity)
        {
            (sender as SearchClientsViewModel).ClientSelected -= _searchClient_clientSelected;
            _booking.Client = selectedClientEntity.Client;
            await _roomChoices.AssignRoomsCommand.ExecuteAsync(_booking);

            LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(this);
            SumUpViewModel sumUpVM = new SumUpViewModel(_navigation, _booking, prevNode);
            NextCalled?.Invoke(null, this);
        }

        private async Task _validateBooking(BookingViewModel bookingVM)
        {
            await _roomChoices.AssignRoomsCommand.ExecuteAsync(_booking);

            LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(this);
            SumUpViewModel sumUpVM = new SumUpViewModel(_navigation, _booking, prevNode);
            NextCalled?.Invoke(null, this);
        }
    }
}
