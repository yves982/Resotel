using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class ClientViewModel : INavigableViewModel, INotifyPropertyChanged, ICloneable
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private LinkedList<INavigableViewModel> _navigation;
        private string _title;

        private DelegateCommand<ClientViewModel> _sumUpCommand;
        private DelegateCommand<ClientViewModel> _bookingCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;

        public ClientEntity Client
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
                if (_clientEntity.FirstName != null || _clientEntity.LastName != null)
                {
                    _title = string.Format("Infos Client: {0} {1}", _clientEntity.FirstName, _clientEntity.LastName);
                } else if( _clientEntity.FirstName == null && _clientEntity.LastName == null)
                {
                    _title = string.Format("Infos Client: {0:HH mm ss}", _clientEntity.Bookings[0].CreationDate);
                }
                return _title;
            }
        }

        public DelegateCommand<ClientViewModel> SumUpCommand
        {
            get
            {
                if(_sumUpCommand == null)
                {
                    _sumUpCommand = new DelegateCommand<ClientViewModel>(_sumUp);
                }
                return _sumUpCommand;
            }
        }

        public DelegateCommand<ClientViewModel> BookingCommand
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

        public ClientViewModel(LinkedList<INavigableViewModel> navigation, ClientEntity clientEntity)
        {
            _pcs = new PropertyChangeSupport(this);
            _clientEntity = clientEntity;
            _clientEntity.PropertyChanged += _clientChanged;
            _navigation = navigation;
            _navigation.AddLast(this);
        }

        public object Clone()
        {
            ClientViewModel clientViewModel = new ClientViewModel(_navigation, _clientEntity);
            return clientViewModel;
        }

        private void _clientChanged(object sender, PropertyChangedEventArgs pcea)
        {
            _pcs.NotifyChange("Title");
        }

        private void _sumUp(ClientViewModel clientVM)
        {
            throw new NotImplementedException();
        }

        private void _booking(ClientViewModel clientVM)
        {
            if(PreviousCalled != null)
            {
                PreviousCalled(this, clientVM);
            }
        }
    }
}
