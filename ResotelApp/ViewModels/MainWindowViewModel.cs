using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ResotelApp.ViewModels.Events;

namespace ResotelApp.ViewModels
{
    class MainWindowViewModel : IViewModel, INotifyPropertyChanged
    {
        private ICollectionView _currentBookingsView;
        private ObservableCollection<Booking> _currentBookings;
        private string _choosenNewBookingTitle;
        private DelegateCommand<object> _addBookingCommand;
        private DelegateCommand<object> _closeBookingCommand;

        public event PropertyChangedEventHandler PropertyChanged;

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
                    _closeBookingCommand = new DelegateCommand<object>(_closeBooking);
                }
                return _closeBookingCommand;
            }
        }

        public ICollectionView CurrentBookings
        {
            get
            {
                if(_currentBookingsView == null)
                {
                    _currentBookingsView = CollectionViewSource.GetDefaultView(_currentBookings);
                }
                return _currentBookingsView;
            }
        }

        public string ChoosenNewBookingTitle
        {
            get { return _choosenNewBookingTitle; }
            set
            {
                _choosenNewBookingTitle = value;
                PropertyChangeSupport.NotifyChange(this, PropertyChanged, nameof(ChoosenNewBookingTitle));
            }
        }

        public string NewBookingTitle
        {
            get
            {
                string newBookingTitle = DateTime.Now.ToString("HH mm ss");
                if(_choosenNewBookingTitle != null)
                {
                    newBookingTitle = _choosenNewBookingTitle;
                }
                PropertyChangeSupport.NotifyChange(this, PropertyChanged, nameof(NewBookingTitle));
                return newBookingTitle;
            }
        }

        public MainWindowViewModel()
        {
            _currentBookings = new ObservableCollection<Booking>();
        }

        private void _addBooking(object ignore)
        {
            // show prompt view
            PromptViewModel promptVM = new PromptViewModel("Question", "Nom d'Onglet");
            promptVM.PromptClosed += _promptClosed;
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(promptVM);

            Booking booking = new Booking();
            _currentBookings.Add(booking);

        }

        private void _closeBooking(object ignore)
        {
            _currentBookings.RemoveAt(_currentBookingsView.CurrentPosition);
        }

        private void _promptClosed(PromptClosedEventArgs pcea)
        {
            if (pcea.PromptResult != null)
            {
                _choosenNewBookingTitle = pcea.PromptResult;
            }
        }
    }

}
