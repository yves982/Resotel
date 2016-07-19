using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ResotelApp.ViewModels.Events;
using ResotelApp.ViewModels.Entities;

namespace ResotelApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICollectionView _currentEntitiesView;
        private ObservableCollection<IEntity> _currentEntities;
        private DelegateCommand<object> _addBookingCommand;
        private DelegateCommand<BookingEntity> _closeBookingCommand;
        private DelegateCommand<object> _addClientCommand;
        private PropertyChangeSupport _pcs;

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
                    _closeBookingCommand = new DelegateCommand<BookingEntity>(_closeBooking);
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

        public MainWindowViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
            _currentEntities = new ObservableCollection<IEntity>();
        }

        private void _addBooking(object ignore)
        {
            PromptViewModel promptVM = new PromptViewModel("Nom du nouvel onglet", "Nom d'Onglet");
            promptVM.PromptClosed += _promptClosed;
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(promptVM);

        }

        private void _addClient(object ignore)
        {
            
        }

        private void _closeBooking(BookingEntity booking)
        {
            _currentEntities.Remove(booking);
        }

        private void _promptClosed(PromptClosedEventArgs pcea)
        {
            string title = _computeTitle(pcea.PromptResult);

            _addBookingToCollection();

            _updateTitle(title);
        }

        private static string _computeTitle(string promptResult)
        {
            if (promptResult == null)
            {
                promptResult = DateTime.Now.ToString("HH mm ss");
            }

            return promptResult;
        }

        private void _addBookingToCollection()
        {
            BookingEntity booking = new BookingEntity(new Booking());
            _currentEntities.Add(booking);
            _currentEntitiesView.MoveCurrentToPosition(_currentEntities.Count - 1);
        }

        private void _updateTitle(string title)
        {
            BookingEntity currentBooking = (BookingEntity)_currentEntities[_currentEntitiesView.CurrentPosition];
            currentBooking.Title = title;
        }
    }

}
