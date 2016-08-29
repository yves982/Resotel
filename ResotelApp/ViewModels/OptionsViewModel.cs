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
        private ObservableCollection<OptionChoiceEntity> _availableOptions;
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
        

        public OptionsViewModel(Booking booking, IEnumerable<OptionChoiceEntity> options)
        {
            _pcs = new PropertyChangeSupport(this);
            _availableOptions = new ObservableCollection<OptionChoiceEntity>(options);
            _availableOptionsView = CollectionViewProvider.Provider(_availableOptions);
            _booking = booking;
            _availableOptionsView.CollectionChanged += _optionChanged;
        }

        private void _optionChanged(object sender, NotifyCollectionChangedEventArgs nccea)
        {
            _booking.OptionChoices.Clear();
            foreach(OptionChoiceEntity optChoiceEntity in _availableOptions)
            {
                if(optChoiceEntity.Taken)
                {
                    _booking.OptionChoices.Add(optChoiceEntity.OptionChoice);
                }
            }
        }
    }
}
