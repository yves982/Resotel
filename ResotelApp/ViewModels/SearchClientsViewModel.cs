using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class SearchClientsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private string _title;
        private ObservableCollection<SearchClientViewModel> _searchClientVMs;
        private ICollectionView _searchClientVMsView;
        private ICollectionViewSource _searchClientVMsSource;
        private string _searchedClient;
        private DelegateCommand<SearchClientsViewModel> _selectClientCommand;
        private ClientEntity _subClientSelected;

        public string Title
        {
            get { return _title; }
        }

        public ICollectionView SearchClientVMsView
        {
            get { return _searchClientVMsView; }
        }

        public string SearchedClient
        {
            get { return _searchedClient; }
            set
            {
                _searchedClient = value;
                _pcs.NotifyChange();
                _searchClientVMsView.Refresh();
            }
        }

        

        public ICommand SelectClientCommand
        {
            get
            {
                if(_selectClientCommand == null)
                {
                    _selectClientCommand = new DelegateCommand<SearchClientsViewModel>(_selectClient, false);
                }
                return _selectClientCommand;
            }
        }

        public bool? ShouldClose { get; set; }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }
       
        public event EventHandler<ClientEntity> ClientSelected;

        public SearchClientsViewModel(List<ClientEntity> clientEntities, bool reservationMode = false)
        {
            _pcs = new PropertyChangeSupport(this);
            _title = "Resotel - Recherche de client";

            if(reservationMode)
            {
                _title = "Resotel - Recherche de réservation";
            }

            _searchClientVMs = new ObservableCollection<SearchClientViewModel>();
            _searchClientVMsSource = CollectionViewProvider.Provider(_searchClientVMs);
            _searchClientVMsView = _searchClientVMsSource.View;
            _searchClientVMsView.Filter = _filterClientNameOrFirstName;
            _searchClientVMsView.CurrentChanged += _client_selected;

            HashSet<string> clientKeys = new HashSet<string>();

            foreach(ClientEntity clientEntity in clientEntities)
            {
                if (clientKeys.Add($"{clientEntity.FirstName}{clientEntity.LastName}"))
                {
                    SearchClientViewModel clientSearchVM = new SearchClientViewModel(clientEntity, clientEntities);
                    _searchClientVMs.Add(clientSearchVM);
                    clientSearchVM.ClientSelected += _subClient_selected;
                }
            }
        }

        ~SearchClientsViewModel()
        {
            foreach(SearchClientViewModel searchClientVM in _searchClientVMs)
            {
                searchClientVM.ClientSelected -= _subClient_selected;
            }
            _searchClientVMsView.CurrentChanged -= _client_selected;
        }

        private bool _filterClientNameOrFirstName(object item)
        {
            SearchClientViewModel clientSearchVM = item as SearchClientViewModel;
            bool mustShow = false;

            if(string.IsNullOrEmpty(_searchedClient) ||
                clientSearchVM.ClientEntity.FirstName.Contains(_searchedClient) || 
                clientSearchVM.ClientEntity.LastName.Contains(_searchedClient)
            )
            {
                mustShow = true;
            }
            return mustShow;
        }

        private void _selectClient(SearchClientsViewModel searchClientsVM)
        {
            ClientEntity selectedClient = null;
            if (SearchClientVMsView.CurrentPosition != -1)
            {
                selectedClient = (SearchClientVMsView.CurrentItem as SearchClientViewModel).ClientEntity;
            } else
            {
                selectedClient = _subClientSelected;
            }
            ClientSelected?.Invoke(this, selectedClient);
            ShouldClose = true;
            _pcs.NotifyChange(nameof(ShouldClose));
    }

        private void _client_selected(object sender, EventArgs e)
        {
            bool nothingSelected = _searchClientVMsView.CurrentPosition == -1;
            bool somethingSelected = !nothingSelected;
            bool canExecute = _selectClientCommand.CanExecute(null);
            bool cannotExecute = !canExecute;

            if ( 
                    (nothingSelected && canExecute) ||
                    (somethingSelected && cannotExecute)
               )
            {
                _selectClientCommand.ChangeCanExecute();
            }
        }

        private void _subClient_selected(object sender, ClientEntity selectedClientEntity)
        {
            bool canExecute = _selectClientCommand.CanExecute(null);
            bool cannotExecute = !canExecute;

            _subClientSelected = selectedClientEntity;
            if(
                (selectedClientEntity != null && cannotExecute) ||
                (selectedClientEntity == null && canExecute)
              )
            {
                _selectClientCommand.ChangeCanExecute();
            }
           
        }
    }
}
