using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ResotelApp.ViewModels.Entities
{
    class RoomEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private Room _room;
        private ObservableCollection<OptionEntity> _optionEntities;
        private ICollectionView _optionEntitiesView;
        private ICollectionViewSource _optionEntitiesSource;

        private ObservableCollection<BookingEntity> _bookingEntities;
        private ICollectionView _bookingEntitiesView;
        private ICollectionViewSource _bookingEntitiesSource;

        private ObservableCollection<PackEntity> _availablePackEntities;
        private ICollectionView _availablePackEntitiesView;
        private ICollectionViewSource _availablePackEntitiesSource;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public int Id
        {
            get { return _room.Id; }
            set
            {
                _room.Id = value;
                _pcs.NotifyChange();
            }
        }

        public int Stage
        {
            get { return _room.Stage; }
            set
            {
                _room.Stage = value;
                _pcs.NotifyChange();
            }
        }

        public int Capacity
        {
            get { return _room.Capacity; }
            set
            {
                _room.Capacity = value;
                _pcs.NotifyChange();
            }
        }

        public ObservableCollection<OptionEntity> Options
        {
            get
            {
                if (_optionEntities == null)
                {
                    _optionEntities = new ObservableCollection<OptionEntity>();
                    foreach (Option opt in _room.Options)
                    {
                        OptionEntity optEntity = new OptionEntity(opt);
                        _optionEntities.Add(optEntity);
                    }
                    _optionEntitiesSource = CollectionViewProvider.Provider(_optionEntities);
                    _optionEntitiesView = _availablePackEntitiesSource.View;
                }
                return _optionEntities;
            }
            set
            {
                if (value != null)
                {
                    _optionEntities = new ObservableCollection<OptionEntity>(value);
                    _room.Options.Clear();
                    foreach (OptionEntity optEntity in value)
                    {
                        _room.Options.Add(optEntity.Option);
                    }
                    _optionEntities = value;
                    _optionEntitiesSource = CollectionViewProvider.Provider(_optionEntities);
                    _optionEntitiesView = _optionEntitiesSource.View;
                    _pcs.NotifyChange();
                }
            }
        }
        public ICollectionView OptionsView
        {
            get { return _optionEntitiesView; }
        }

        public ObservableCollection<BookingEntity> Bookings
        {
            get
            {
                if (_bookingEntities == null)
                {
                    _bookingEntities = new ObservableCollection<BookingEntity>();
                    foreach (Booking booking in _room.Bookings)
                    {
                        BookingEntity bookingEntity = new BookingEntity(booking);
                        _bookingEntities.Add(bookingEntity);
                    }
                    _bookingEntitiesSource = CollectionViewProvider.Provider(_bookingEntities);
                    _bookingEntitiesView = _bookingEntitiesSource.View;
                }
                return _bookingEntities;
            }
            set
            {
                if (value != null)
                {
                    _bookingEntities = new ObservableCollection<BookingEntity>(value);
                    _room.Bookings.Clear();
                    foreach (BookingEntity bookingEntity in value)
                    {
                        _room.Bookings.Add(bookingEntity.Booking);
                    }
                    _bookingEntities = value;
                    _bookingEntitiesSource = CollectionViewProvider.Provider(_bookingEntities);
                    _bookingEntitiesView = _bookingEntitiesSource.View;
                    _pcs.NotifyChange();
                }

            }
        }
        public ICollectionView BookingsView
        {
            get { return _bookingEntitiesView; }
        }

        public ObservableCollection<PackEntity> AvailablePacks
        {
            get
            {
                if (_availablePackEntities == null)
                {
                    _availablePackEntities = new ObservableCollection<PackEntity>();
                    foreach (Pack pack in _room.AvailablePacks)
                    {
                        PackEntity packEntity = new PackEntity(pack);
                        _availablePackEntities.Add(packEntity);
                    }
                    _availablePackEntitiesSource = CollectionViewProvider.Provider(_availablePackEntities);
                    _availablePackEntitiesView = _availablePackEntitiesSource.View;
                }
                return _availablePackEntities;
            }
            set
            {
                if (value != null)
                {
                    _availablePackEntities = new ObservableCollection<PackEntity>(value);
                    _room.AvailablePacks.Clear();
                    foreach (PackEntity packEntity in value)
                    {
                        _room.AvailablePacks.Add(packEntity.Pack);
                    }
                    _availablePackEntities = value;
                    _availablePackEntitiesSource = CollectionViewProvider.Provider(_availablePackEntities);
                    _availablePackEntitiesView = _availablePackEntitiesSource.View;
                    _pcs.NotifyChange();
                }
            }
        }
        public ICollectionView AvailablePacksView
        {
            get { return _availablePackEntitiesView; }
        }

        public bool IsCleaned
        {
            get { return _room.IsCleaned; }
            set
            {
                _room.IsCleaned = value;
                _pcs.NotifyChange();
            }
        }


        public BedKind BedKind
        {
            get { return _room.BedKind; }
            set
            {
                _room.BedKind = value;
                _pcs.NotifyChange();
            }
        }


        public double Price
        {
            get { return _room.Price; }
            set
            {
                _room.Price = value;
                _pcs.NotifyChange();
            }
        }

        public RoomKind Kind
        {
            get { return _room.Kind; }
        }

        public string Label
        {
            get { return _room.Label; }
        }

        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_room).Error; }
        }

        public Room Room { get; internal set; }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ((IDataErrorInfo)_room)[columnName]; }
        }



        public RoomEntity(Room room)
        {
            _pcs = new PropertyChangeSupport(this);
            _room = room;

        }
    }
}