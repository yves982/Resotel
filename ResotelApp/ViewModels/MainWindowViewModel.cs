using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICollectionView _currentEntitiesView;
        private ObservableCollection<INavigableViewModel> _currentEntities;
        private List<OptionEntity> _availableOptions;
        private List<RoomChoiceEntity> _defaultAvailableRoomChoices;
        private PropertyChangeSupport _pcs;
        private string _title;
        private User _user;
        private LinkedList<INavigableViewModel> _navigation;


        private DelegateCommand<object> _addBookingCommand;
        private DelegateCommand<IEntity> _closeBookingCommand;
        private DelegateCommand<object> _addClientCommand;
        private DelegateCommandAsync<object> _loadCommand;
        private DelegateCommand<BookingViewModel> _nextCommand;
        private DelegateCommand<BookingViewModel> _prevCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public ICommand AddBookingCommand
        {
            get
            {
                if(_addBookingCommand == null)
                {
                    _addBookingCommand = new DelegateCommand<object>(_addBooking);
                }
                return _addBookingCommand;
            }
        }

        public ICommand CloseBookingCommand
        {
            get
            {
                if(_closeBookingCommand == null)
                {
                    _closeBookingCommand = new DelegateCommand<IEntity>(_closeBooking);
                }
                return _closeBookingCommand;
            }
        }

        public ICommand AddClientCommand
        {
            get
            {
                if(_addClientCommand == null)
                {
                    _addClientCommand = new DelegateCommand<object>(_addClient);
                }
                return _addClientCommand;
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                if(_loadCommand == null)
                {
                    _loadCommand = new DelegateCommandAsync<object>(_load);
                }
                return _loadCommand;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                if(_nextCommand == null)
                {
                    _nextCommand = new DelegateCommand<BookingViewModel>(_next);
                }
                return _nextCommand;
            }
        }

        public ICommand PrevCommand
        {
            get
            {
                if (_prevCommand == null)
                {
                    _prevCommand = new DelegateCommand<BookingViewModel>(_prev);
                }
                return _prevCommand;
            }
        }

        public ICollectionView CurrentEntitiesView
        {
            get
            {
                if(_currentEntitiesView == null)
                {
                    _currentEntitiesView = CollectionViewSource.GetDefaultView(_currentEntities);
                }
                return _currentEntitiesView;
            }
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

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                _pcs.NotifyChange();
            }
        }


        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }
        
            public MainWindowViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
            _currentEntities = new ObservableCollection<INavigableViewModel>();
            _title = "Resotel - Facturation";
            _availableOptions = new List<OptionEntity>();
            _defaultAvailableRoomChoices = new List<RoomChoiceEntity>();
            _navigation = new LinkedList<INavigableViewModel>();
        }

        private void _addBooking(object ignore)
        {
            Booking booking = new Booking();
            booking.CreationDate = DateTime.Now;
            booking.Dates.Start = DateTime.Now;
            booking.Dates.End = DateTime.Now.AddDays(1.0);
            List<OptionEntity> availableOptions = _cloneOptions();
            List<RoomChoiceEntity> availableRoomsChoices = _cloneRoomsChoices();
            BookingViewModel bookingVM = new BookingViewModel(_navigation, booking, availableOptions, availableRoomsChoices);
            _navigation = bookingVM.Navigation;
            _currentEntities.Add(bookingVM);
            _currentEntitiesView.MoveCurrentToPosition(_currentEntities.Count - 1);
            bookingVM.NextCalled += _nextCalled;
            bookingVM.PreviousCalled += _prevCalled;
        }

        private void _addClient(object ignore)
        {
            
        }

        private void _closeBooking(IEntity closedEntity)
        {
            int entityIndex = -1;
            int i = 0;
            foreach(IEntity entity in _currentEntities)
            {
                if(entity.Equals(closedEntity))
                {
                    entityIndex = i;
                    break;
                }
                i++;
            }
            if(entityIndex != -1)
            {
                _currentEntities.RemoveAt(entityIndex);
            }
        }

        private List<OptionEntity> _cloneOptions()
        {
            List<OptionEntity> options = new List<OptionEntity>();
            foreach(OptionEntity opt in _availableOptions)
            {
                options.Add((OptionEntity)((ICloneable)opt).Clone());
            }
            return options;
        }

        private List<RoomChoiceEntity> _cloneRoomsChoices()
        {
            List<RoomChoiceEntity> roomsChoices = new List<RoomChoiceEntity>();
            foreach(RoomChoiceEntity roomChoice in _defaultAvailableRoomChoices)
            {
                roomsChoices.Add((RoomChoiceEntity)((ICloneable)roomChoice).Clone());
            }
            return roomsChoices;
        }

        private async Task _load(object ignore)
        {
            
            DateRange defaultBookingDates = new DateRange
            {
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1.0)
            };

            List<Option> availableOptions = await OptionRepository.GetAvailablesBetweenAsync(defaultBookingDates);
            List<Room> defaultAvailableRooms = await RoomRepository.GetAvailablesBetweenAsync(defaultBookingDates);

            foreach(Option opt in availableOptions)
            {
                _availableOptions.Add(new OptionEntity(opt));
            }

            HashSet<BedKind> bedKinds = new HashSet<BedKind>();
            foreach(Room room in defaultAvailableRooms)
            {
                if(!bedKinds.Contains(room.BedKind))
                {
                    bedKinds.Add(room.BedKind);
                    string imageFullPath = string.Format("/Resources/BedKind_{0}.png", room.BedKind.ToString());
                    RoomChoiceEntity roomChoice = new RoomChoiceEntity(imageFullPath, room.BedKind);
                    _defaultAvailableRoomChoices.Add(roomChoice);
                }
            }
        }


        private void _nextCalled(object sender, INavigableViewModel navigableVM)
        {
            _next(navigableVM);
        }

        private void _prevCalled(object sender, INavigableViewModel navigableVM)
        {
            _prev(navigableVM);
        }

        private void _next(INavigableViewModel navigableVM)
        {
            LinkedListNode<INavigableViewModel> currentNode = _navigation.Find(navigableVM);
            if(currentNode.Next != null)
            {
                currentNode = currentNode.Next;
                _currentEntities[_currentEntitiesView.CurrentPosition] = currentNode.Value;
                currentNode.Value.NextCalled += _nextCalled;
                currentNode.Value.PreviousCalled += _prevCalled;
            }
        }

        private void _prev(INavigableViewModel navigableVM)
        {
            LinkedListNode<INavigableViewModel> currentNode = _navigation.Find(navigableVM);
            if (currentNode.Previous != null)
            {
                currentNode = currentNode.Previous;
                _currentEntities[_currentEntitiesView.CurrentPosition] = currentNode.Value;
            }
        }
    }

}
