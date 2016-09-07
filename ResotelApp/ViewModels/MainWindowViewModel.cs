using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICollectionView _currentEntitiesView;
        private ObservableCollection<INavigableViewModel> _currentEntities;
        private PropertyChangeSupport _pcs;
        private string _title;
        private UserEntity _user;
        private LinkedList<INavigableViewModel> _navigation;
        private string _message;
        private MessageKind _messageKind;


        private DelegateCommand<object> _addBookingCommand;
        private DelegateCommand<IEntity> _closeBookingCommand;
        private DelegateCommand<object> _addClientCommand;
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


        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }
        
        public string Message
        {
            get { return _message; }
            set
            {
                if(value != null && value[0] == '#')
                {
                    MessageKind = MessageKind.Error;
                    _message = value.Substring(1);
                } else
                {
                    MessageKind = MessageKind.Standard;
                    _message = value;
                }
                _pcs.NotifyChange();
            }
        }

        public MessageKind MessageKind
        {
            get { return _messageKind; }
            set
            {
                _messageKind = value;
                _pcs.NotifyChange();
            }
        }

        public MainWindowViewModel(UserEntity user)
        {
            _pcs = new PropertyChangeSupport(this);
            _user = user;
            _currentEntities = new ObservableCollection<INavigableViewModel>();
            _title = "Resotel - Facturation";
            _navigation = new LinkedList<INavigableViewModel>();
        }

        private void _addBooking(object ignore)
        {
            Booking booking = new Booking();
            booking.CreationDate = DateTime.Now;
            booking.Dates.Start = DateTime.Now;
            booking.Dates.End = DateTime.Now.AddDays(1.0);
            BookingViewModel bookingVM = new BookingViewModel(_navigation, booking);
            _navigation = bookingVM.Navigation;
            _currentEntities.Add(bookingVM);
            _currentEntitiesView.MoveCurrentToPosition(_currentEntities.Count - 1);
            bookingVM.NextCalled += _nextCalled;
            bookingVM.PreviousCalled += _prevCalled;
            bookingVM.MessageReceived += _messageReceived;
        }

        private void _addClient(object ignore)
        {
            Client newClient = new Client();
            ClientEntity newClientEntity = new ClientEntity(newClient);
            ClientViewModel clientVM = new ClientViewModel(_navigation, newClientEntity, true);
            _currentEntities.Add(clientVM);
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
            if(entityIndex != -1 && closedEntity is INavigableViewModel)
            {
                INavigableViewModel bookingVM = closedEntity as INavigableViewModel;
                bookingVM.NextCalled -= _nextCalled;
                bookingVM.PreviousCalled -= _prevCalled;
                bookingVM.MessageReceived -= _messageReceived;
                _currentEntities.RemoveAt(entityIndex);
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

        private void _nodeShutdown(object sender, INavigableViewModel navigableVM)
        {
            _node_shutdown(navigableVM);
        }

        private void _messageReceived(object sender, string message)
        {
            Message = message;
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
                currentNode.Value.Shutdown += _nodeShutdown;
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

        private void _node_shutdown(INavigableViewModel navigableVM)
        {
            if (navigableVM != null)
            {
                navigableVM.NextCalled -= _nextCalled;
                navigableVM.PreviousCalled -= _prevCalled;
                navigableVM.Shutdown -= _nodeShutdown;
            }
        }
    }

}
