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
    class ClientViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private LinkedList<INavigableViewModel> _navigation;
        private string _title;
        private bool _clientMode;
        private bool _bookingMode;
        private BookingViewModel _currentBookingVM;

        private DelegateCommand<ClientViewModel> _sumUpCommand;
        private DelegateCommand<ClientViewModel> _bookingCommand;
        private DelegateCommandAsync<ClientViewModel> _saveClientCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;
        public event EventHandler<string> MessageReceived;

        public ClientEntity ClientEntity
        {
            get { return _clientEntity; }
        }

        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }

        public string Title
        {
            get
            {
                if (_bookingMode)
                {
                    if (_clientEntity.FirstName != null || _clientEntity.LastName != null)
                    {
                        _title = string.Format("Infos Client: {0} {1}", _clientEntity.FirstName, _clientEntity.LastName);
                    }
                    else if (_clientEntity.FirstName == null && _clientEntity.LastName == null)
                    {
                        _title = string.Format("Infos Client: {0:HH mm ss}", _clientEntity.Bookings[0].CreationDate);
                    }
                }
                else if(_clientMode) 
                {
                    if (_clientEntity.FirstName == null && _clientEntity.LastName == null 
                        && _clientEntity.Id == 0)
                    {
                        _title = string.Format("Ajout Client: {0: HH mm ss}", DateTime.Now);
                    } else if(_clientEntity.Id == 0)
                    {
                        _title = string.Format("Ajout Client: {0} {1}", _clientEntity.FirstName, _clientEntity.LastName);
                    } else if(_clientEntity.Id > 0)
                    {
                        _title = string.Format("Edition Client: {0} {1}", _clientEntity.FirstName, _clientEntity.LastName);
                    }
                }
                return _title;
            }
        }

        public bool ClientMode
        {
            get { return _clientMode; }
        }

        public bool BookingMode
        {
            get { return _bookingMode; }
        }

        public BookingViewModel CurrentBookingVM
        {
            get { return _currentBookingVM; }
        }

        public ICommand SumUpCommand
        {
            get { return _sumUpCommand; }
        }

        public ICommand BookingCommand
        {
            get { return _bookingCommand; }
        }

        public ICommand SaveClientCommand
        {
            get { return _saveClientCommand; }
        }

        public ClientViewModel(LinkedList<INavigableViewModel> navigation, ClientEntity clientEntity, LinkedListNode<INavigableViewModel> prevNode = null)
        {
            _pcs = new PropertyChangeSupport(this);
            _navigation = navigation;
            _clientEntity = clientEntity;
            _clientEntity.PropertyChanged += _clientChanged;
            _clientMode = prevNode == null;
            _bookingMode = !_clientMode;
            
            if (_bookingMode)
            {
                _currentBookingVM = prevNode.Value as BookingViewModel;
            }

            _sumUpCommand = new DelegateCommand<ClientViewModel>(_sumUp);
            _bookingCommand = new DelegateCommand<ClientViewModel>(_booking);
            _saveClientCommand = new DelegateCommandAsync<ClientViewModel>(_saveClient);

            _unlockSaveAndSumUpIfNeeded();

            if (!_bookingMode)
            {
                _navigation.AddLast(this);
            }else
            {
                _navigation.AddAfter(prevNode, this);
            }
        }

        ~ClientViewModel()
        {
            _clientEntity.PropertyChanged -= _clientChanged;
            Shutdown?.Invoke(null, this);
        }

        private void _clientChanged(object sender, PropertyChangedEventArgs pcea)
        {
            _unlockSaveAndSumUpIfNeeded();
            _pcs.NotifyChange("Title");
        }

        private void _unlockSaveAndSumUpIfNeeded()
        {
            bool canSave = _saveClientCommand.CanExecute(null);
            bool canSumUp = _sumUpCommand.CanExecute(null);
            bool isValid = ((IDataErrorInfo)_clientEntity).Error == null;

            if( (canSave && !isValid) ||
                (!canSave && isValid)
            )
            {
                _saveClientCommand.ChangeCanExecute();
            }

            if((canSumUp && !isValid) ||
                (!canSumUp && isValid)
            )
            {
                _sumUpCommand.ChangeCanExecute();
            }
        }

        private void _sumUp(ClientViewModel clientVM)
        {           
            int bookingCnt = clientVM.ClientEntity.Bookings.Count;
            Booking booking = clientVM.CurrentBookingVM.Booking;
            if (!(_navigation.Last.Value is SumUpViewModel))
            {
                LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(this);
                SumUpViewModel sumUpVM = new SumUpViewModel(_navigation, clientVM.CurrentBookingVM.Booking, prevNode);
            }
            NextCalled?.Invoke(null, clientVM);
        }

        private void _booking(ClientViewModel clientVM)
        {
            PreviousCalled?.Invoke(null, clientVM);
        }

        private async Task _saveClient(ClientViewModel clientVM)
        {
            if( clientVM == null || clientVM.ClientEntity == null)
            {
                throw new ArgumentException("L'argument ne peut être null et sa propriété ClientEntity non plus", nameof(clientVM));
            }
            await ClientRepository.Save(clientVM.ClientEntity.Client);
            string action = "créé";
            if(clientVM.ClientEntity.Id != 0)
            {
                action = "mis à jour";
            }
            PromptViewModel promptVM = new PromptViewModel("Action réussie", 
                $"Le client {clientVM.ClientEntity.FirstName} {clientVM.ClientEntity.LastName} a été {action}.",
                false
            );
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(promptVM);
        }
    }
}
