using ResotelApp.DAL;
using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public event EventHandler<string> MessageReceived;

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
                if(_options != null)
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
            _clientEntity = new ClientEntity(_booking.Client);
            _clientEntity.Bookings.Add(_booking);
            _computeTitle(_clientEntity);
            _clientEntity.PropertyChanged += _clientChanged;
            _navigation = navigation;
            _navigation.AddLast(this);
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

        private async void _parameters_defined(object sender, BookingParametersViewModel bookingParametersVM)
        {
            ParametersValidated = true;
            _newClientCommand.ChangeCanExecute();
            _searchClientCommand.ChangeCanExecute();

            Options = await OptionsViewModel.CreateAsync(bookingParametersVM.DateRangeEntity.DateRange);
            RoomChoices = await RoomsChoiceViewModel.CreateAsync(bookingParametersVM.DateRangeEntity.DateRange);
            RoomChoices.AvailableRoomChoiceEntitiesView.Filter = _mustShowRoomChoice;
        }

        private bool _mustShowRoomChoice(object item)
        {
            if(! (item is RoomChoiceEntity) )
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
            _booking.Dates = _parameters.DateRangeEntity.DateRange;

            if (_newClientCommand.CanExecute(null))
            {
                ParametersValidated = false;
                _newClientCommand.ChangeCanExecute();
                _searchClientCommand.ChangeCanExecute();
                _parameters.ChangeValidateCanExecute();
                MessageReceived?.Invoke(null, null);
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

        private void _optionsChanged(object sender, OptionChoiceEntity optChoiceEntity)
        {
            if (optChoiceEntity.Taken && _booking.OptionChoices.Find( optC => optC.Option.Id == optChoiceEntity.OptionChoice.Option.Id ) == null)
            {
                _booking.OptionChoices.Add(optChoiceEntity.OptionChoice);
            } else if(!optChoiceEntity.Taken)
            {
                _booking.OptionChoices.Remove(optChoiceEntity.OptionChoice);
            }

            bool previouslyHadBabyRoom = hasAvailableBabyRoom();

            RoomChoices.Update(optChoiceEntity);

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

            else if(_parameters.BabiesCount > 0 && hasBabyRoom && !previouslyHadBabyRoom)
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
            ClientViewModel clientVM = new ClientViewModel(_navigation, _clientEntity);
            await _roomChoices.AssignRoomsCommand.ExecuteAsync(_booking);
            
            if(_clientEntity.Bookings != null && _clientEntity.Bookings.Count>0)
            {
                int babiesRooms = 0;
                int totalAdultCapacity = 0;

                foreach(Room room in _booking.Rooms)
                {
                    if(room.Kind.Equals(RoomKind.DoubleWithBaby))
                    {
                        babiesRooms++;
                    }

                    totalAdultCapacity += room.Capacity;
                }

                if(babiesRooms < _booking.BabiesCount || totalAdultCapacity < _booking.AdultsCount)
                {
                    PromptViewModel insufficientRoomsPromptVM = new PromptViewModel("Erreur", 
                        $"Il n'y a pas assez de chambre pour accueillir {_booking.BabiesCount} bébés et {_booking.AdultsCount} adultes.",
                        false
                    );
                    ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(insufficientRoomsPromptVM);
                }
                else
                {
                    NextCalled?.Invoke(null, bookingVM);
                }
            }
        }

        private async Task _searchClient(BookingViewModel bookingVM)
        {
            throw new NotImplementedException();
        }
    }
}
