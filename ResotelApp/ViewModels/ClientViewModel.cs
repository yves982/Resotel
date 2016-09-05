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
    class ClientViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private LinkedList<INavigableViewModel> _navigation;
        private string _title;
        private bool _clientMode;
        private bool _bookingMode;
        private BookingViewModel _currentBookingVM;

        private DelegateCommandAsync<ClientViewModel> _sumUpCommand;
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
                    if (_clientEntity.FirstName == null && _clientEntity.LastName == null)
                    {
                        _title = string.Format("Ajout Client: {0: HH mm ss}", DateTime.Now);
                    } else
                    {
                        _title = string.Format("Ajout Client: {0} {1}", _clientEntity.FirstName, _clientEntity.LastName);
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
            get
            {
                if(_sumUpCommand == null)
                {
                    _sumUpCommand = new DelegateCommandAsync<ClientViewModel>(_sumUp);
                }
                return _sumUpCommand;
            }
        }

        public ICommand BookingCommand
        {
            get
            {
                if(_bookingCommand == null)
                {
                    _bookingCommand = new DelegateCommand<ClientViewModel>(_booking);
                }
                return _bookingCommand;
            }
        }

        public ICommand SaveClientCommand
        {
            get
            {
                if (_saveClientCommand == null)
                {
                    _saveClientCommand = new DelegateCommandAsync<ClientViewModel>(_saveClient);
                }
                return _saveClientCommand;
            }
        }

        public ClientViewModel(LinkedList<INavigableViewModel> navigation, ClientEntity clientEntity, bool clientMode = false)
        {
            _pcs = new PropertyChangeSupport(this);
            _clientEntity = clientEntity;
            _clientEntity.PropertyChanged += _clientChanged;
            _clientMode = clientMode;
            _bookingMode = !clientMode;
            _navigation = navigation;
            _navigation.AddLast(this);
            if (_bookingMode)
            {
                _currentBookingVM = _navigation.Last.Previous.Value as BookingViewModel;
            }
        }

        ~ClientViewModel()
        {
            _clientEntity.PropertyChanged -= _clientChanged;
            Shutdown?.Invoke(null, this);
        }

        private void _clientChanged(object sender, PropertyChangedEventArgs pcea)
        {
            _pcs.NotifyChange("Title");
        }

        private async Task _sumUp(ClientViewModel clientVM)
        {
            await clientVM.CurrentBookingVM.Save();
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
            await ClientRepository.SaveNewClient(clientVM.ClientEntity.Client);
            PromptViewModel promptVM = new PromptViewModel("Action réussie", 
                $"Le client {clientVM.ClientEntity.FirstName} {clientVM.ClientEntity.LastName}",
                false
            );
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(promptVM);
        }
    }
}
