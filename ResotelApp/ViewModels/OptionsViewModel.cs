using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Events;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class OptionsViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ICollectionView _availableOptionChoiceEntitiesView;
        private ObservableCollection<OptionChoiceEntity> _availableOptionChoiceEntities;
        private List<OptionChoiceEntity> _choosenOptionChoiceEntities;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public event EventHandler<OptionChoiceEntity> OptionChanged;

        public ICollectionView AvailableOptionChoiceEntitiesView
        {
            get
            {
                return _availableOptionChoiceEntitiesView;
            }
            set
            {
                _availableOptionChoiceEntitiesView = value;
                _pcs.NotifyChange();
            }
        }

        public IList<OptionChoiceEntity> ChoosenOptionChoiceEntities
        {
            get { return _choosenOptionChoiceEntities; }
        }
        

        private OptionsViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
            _availableOptionChoiceEntities = new ObservableCollection<OptionChoiceEntity>();
            _availableOptionChoiceEntitiesView = CollectionViewProvider.Provider(_availableOptionChoiceEntities);
        }

        ~ OptionsViewModel()
        {
            foreach (OptionChoiceEntity optChoiceEntity in _availableOptionChoiceEntitiesView)
            {
                optChoiceEntity.PropertyChanged -= _optionChanged;
            }
        }

        public static async Task<OptionsViewModel> CreateAsync(DateRange dates)
        {
            OptionsViewModel newInstance = new OptionsViewModel();
            List<Option> availableOptions = await OptionRepository.GetAvailablesBetweenAsync(dates);
            foreach (Option opt in availableOptions)
            {
                OptionChoice optChoice = new OptionChoice
                {
                    Option = opt,
                    TakenDates = (DateRange)((ICloneable)dates).Clone()
                };
                optChoice.TakenDates.Start = optChoice.TakenDates.Start.Date;
                OptionChoiceEntity optChoiceEntity = new OptionChoiceEntity(optChoice);
                newInstance._availableOptionChoiceEntities.Add(optChoiceEntity);
            }

            foreach (OptionChoiceEntity optChoiceEntity in newInstance._availableOptionChoiceEntities)
            {
                optChoiceEntity.PropertyChanged += newInstance._optionChanged;
            }
            return newInstance;
        }

        private void _optionChanged(object sender, PropertyChangedEventArgs pcea)
        {
            if(!(sender is OptionChoiceEntity))
            {
                throw new InvalidOperationException();
            }
            
            OptionChoiceEntity optChoiceEntity = sender as OptionChoiceEntity;

            OptionChanged?.Invoke(null, optChoiceEntity);
        }

        private bool _isChoosen(object item)
        {
            if(!(item is OptionChoiceEntity))
            {
                throw new InvalidOperationException("Seules les OptionChoiceEntity peuvent êtres filtrées (OptionsViewModel). Cette exception est critique.");
            }
            OptionChoiceEntity optChoiceEntity = item as OptionChoiceEntity;
            return optChoiceEntity.Taken;
        }
    }
}
