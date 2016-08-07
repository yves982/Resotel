using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ResotelApp.ViewModels
{
    class OptionsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ICollectionView _availableOptionsView;
        private ObservableCollection<OptionEntity> _availableOptions;
        private Booking _booking;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }


        public ICollectionView AvailableOptionsView
        {
            get
            {
                return _availableOptionsView;
            }
            set
            {
                _availableOptionsView = value;
                _pcs.NotifyChange();
            }
        }
        

        public OptionsViewModel(Booking booking, IEnumerable<OptionEntity> options)
        {
            _pcs = new PropertyChangeSupport(this);
            _availableOptions = new ObservableCollection<OptionEntity>(options);
            _availableOptionsView = CollectionViewProvider.Provider(_availableOptions);
            _booking = booking;
            _availableOptionsView.CollectionChanged += _optionChanged;
        }

        private void _optionChanged(object sender, NotifyCollectionChangedEventArgs nccea)
        {
            _booking.Options.Clear();
            foreach(OptionEntity option in _availableOptions)
            {
                if(option.Taken)
                {
                    _booking.Options.Add(option.Option);
                }
            }
        }
    }
}
