using ResotelApp.ViewModels.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using ResotelApp.ViewModels.Entities;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class SearchClientViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private ClientEntity _clientEntity;
        private bool _needsCount;
        private int _foundClientsCount;
        private ObservableCollection<ClientEntity> _subClientEntities;
        private ICollectionView _subClientEntitiesView;
        private bool _displayMoreToggled;
        private DelegateCommand<SearchClientViewModel> _displayMoreCommand;


        public event EventHandler<ClientEntity> ClientSelected;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public ClientEntity ClientEntity
        {
            get { return _clientEntity; }
        }

        public bool NeedsCount
        {
            get { return _needsCount; }
        }

        public int FoundClientsCount
        {
            get { return _foundClientsCount; }
        }

        public ICollectionView SubClientEntitiesView
        {
            get { return _subClientEntitiesView; }
        }

        public string DisplayMoreImage
        {
            get { return !_displayMoreToggled ? "/Resources/displayMoreIcon.png" : "/Resources/displayLessIcon.png"; }
        }

        public bool ShowSubs
        {
            get { return _displayMoreToggled; }
        }

        public ICommand DisplayMoreCommand
        {
            get
            {
                if (_displayMoreCommand == null)
                {
                    _displayMoreCommand = new DelegateCommand<SearchClientViewModel>(_displayMore);
                }
                return _displayMoreCommand;
            }
        }

        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_clientEntity).Error; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = null;
                if(columnName == "ClientEntity")
                {
                    error = ((IDataErrorInfo)_clientEntity).Error;
                }
                return error;
            }
        }

        public SearchClientViewModel(ClientEntity clientEntity, IEnumerable<ClientEntity> clientEntities)
        {
            _pcs = new PropertyChangeSupport(this);
            _displayMoreToggled = false;
            _clientEntity = clientEntity;
            _subClientEntities = new ObservableCollection<ClientEntity>();
            _subClientEntitiesView = CollectionViewProvider.Provider(_subClientEntities);
            _subClientEntitiesView.CurrentChanged += _subClientEntitiesView_CurrentChanged;

            foreach(ClientEntity clientE in clientEntities)
            {
                if(clientE.FirstName == _clientEntity.FirstName && clientE.LastName == _clientEntity.LastName)
                {
                    _subClientEntities.Add(clientE);
                }
            }
            
            _needsCount = _subClientEntities.Count > 1;
            _foundClientsCount = _subClientEntities.Count;
        }

        

        private void _displayMore(SearchClientViewModel searchClientsVM)
        {
            _displayMoreToggled = !_displayMoreToggled;
            _pcs.NotifyChange(nameof(ShowSubs));
            _pcs.NotifyChange(nameof(DisplayMoreImage));
        }

        private void _subClientEntitiesView_CurrentChanged(object sender, EventArgs e)
        {
            ClientEntity selectedClientEntity = _subClientEntitiesView.CurrentItem as ClientEntity;
            ClientSelected?.Invoke(null, selectedClientEntity);
        }
    }
}
