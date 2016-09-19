using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels.Entities
{
    class BookingEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private Booking _booking;
        private ClientEntity _clientEntity;
        private PaymentEntity _paymentEntity;

        private ObservableCollection<OptionChoiceEntity> _optionChoicesEntities;
        private ICollectionView _optionChoicesEntitiesView;
        private ICollectionViewSource _optionChoicesEntitiesSource;
        private ObservableCollection<RoomEntity> _roomEntities;
        private ICollectionView _roomEntitiesView;
        private ICollectionViewSource _roomEntitiesSource;
        private ObservableCollection<AppliedPackEntity> _roomPackEntities;
        private ICollectionView _roomPackEntitiesView;
        private ICollectionViewSource _roomPackEntitiesSource;

        private DiscountEntity _optionDiscountEntity;
        private DateRangeEntity _datesEntity;
        
        private OptionChoiceEntity _discountedOptionChoiceEntity;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Booking Booking
        {
            get { return _booking; }
        }

        public int Id
        {
            get { return _booking.Id; }
            set
            {
                _booking.Id = value;
                _pcs.NotifyChange();
            }
        }
        
        public ClientEntity Client
        {
            get { return _clientEntity; }
            set
            {
                _clientEntity = value;
                _booking.Client = _clientEntity.Client;
                _pcs.NotifyChange();
            }
        }

        public DateTime? TerminatedDate
        {
            get { return _booking.TerminatedDate; }
            set
            {
                _booking.TerminatedDate = value;
                _pcs.NotifyChange();
                _pcs.NotifyChange(nameof(State));
            }
        }

        public PaymentEntity Payment
        {
            get { return _paymentEntity; }
            set
            {
                _paymentEntity = value;
                _pcs.NotifyChange();
                _pcs.NotifyChange(nameof(State));
            }
        }

        public ObservableCollection<OptionChoiceEntity> OptionChoices
        {
            get
            {
                if(_optionChoicesEntities == null)
                {
                    _optionChoicesEntities = new ObservableCollection<OptionChoiceEntity>();
                    foreach(OptionChoice optChoice in _booking.OptionChoices)
                    {
                        OptionChoiceEntity optChoiceEntity = new OptionChoiceEntity(_booking, optChoice);
                        _optionChoicesEntities.Add(optChoiceEntity);
                    }
                    _optionChoicesEntitiesSource = CollectionViewProvider.Provider(_optionChoicesEntities);
                    _optionChoicesEntitiesView = _optionChoicesEntitiesSource.View;
                }
                return _optionChoicesEntities;
            }
            set
            {
                if(value != null)
                {
                    _optionChoicesEntities = new ObservableCollection<OptionChoiceEntity>(value);
                    _booking.OptionChoices.Clear();
                    foreach(OptionChoiceEntity optChoiceEntity in value)
                    {
                        _booking.OptionChoices.Add(optChoiceEntity.OptionChoice);
                    }
                    _optionChoicesEntitiesSource = CollectionViewProvider.Provider(_optionChoicesEntities);
                    _optionChoicesEntitiesView = _optionChoicesEntitiesSource.View;
                    _pcs.NotifyChange();
                    _pcs.NotifyChange(nameof(OptionChoicesView));
                }
            }
        }

        public ICollectionView OptionChoicesView
        {
            get { return _optionChoicesEntitiesView; }
        }

        public DateTime CreationDate
        {
            get { return _booking.CreationDate; }
            set
            {
                _booking.CreationDate = value;
                _pcs.NotifyChange();
            }
        }

        public ObservableCollection<RoomEntity> Rooms
        {
            get
            {
                if(_roomEntities == null)
                {
                    _roomEntities = new ObservableCollection<RoomEntity>();
                    foreach(Room room in _booking.Rooms)
                    {
                        RoomEntity roomEntity = new RoomEntity(room);
                        _roomEntities.Add(roomEntity);
                    }
                    _roomEntitiesSource = CollectionViewProvider.Provider(_roomEntities);
                    _roomEntitiesView = _roomEntitiesSource.View;
                }
                return _roomEntities;
            }
            set
            {
                if(value != null)
                {
                    _booking.Rooms.Clear();
                    foreach(RoomEntity roomEntity in value)
                    {
                        _booking.Rooms.Add(roomEntity.Room);
                    }
                }
                _roomEntities = value;
                _roomEntitiesSource = CollectionViewProvider.Provider(_roomEntities);
                _roomEntitiesView = _roomEntitiesSource.View;
                _pcs.NotifyChange();
                _pcs.NotifyChange(nameof(RoomsView));
            }
        }

        public ICollectionView RoomsView
        {
            get
            {
                return _roomEntitiesView;
            }
        }

        public ObservableCollection<AppliedPackEntity> RoomPacks
        {
            get
            {
                if (_roomPackEntities == null)
                {
                    _roomPackEntities = new ObservableCollection<AppliedPackEntity>();
                    foreach (AppliedPack appliedPack in _booking.RoomPacks)
                    {
                        AppliedPackEntity appliedPackEntity = new AppliedPackEntity(appliedPack);
                        _roomPackEntities.Add(appliedPackEntity);
                    }
                    _roomPackEntitiesSource = CollectionViewProvider.Provider(_roomPackEntities);
                    _roomPackEntitiesView = _roomPackEntitiesSource.View;
                }
                return _roomPackEntities;
            }
            set
            {
                if (value != null)
                {
                    _booking.RoomPacks.Clear();
                    foreach (AppliedPackEntity appliedPackEntity in value)
                    {
                        _booking.RoomPacks.Add(appliedPackEntity.AppliedPack);
                    }
                }
                _roomPackEntities = value;
                _roomPackEntitiesSource = CollectionViewProvider.Provider(_roomPackEntities);
                _roomPackEntitiesView = _roomPackEntitiesSource.View;
                _pcs.NotifyChange();
                _pcs.NotifyChange(nameof(RoomPacksView));
            }
        }

        public ICollectionView RoomPacksView
        {
            get { return _roomPackEntitiesView; }
        }

        public DateRangeEntity Dates
        {
            get { return _datesEntity; }
            set
            {
                _datesEntity = value;
                _booking.Dates = _datesEntity.DateRange;
                _pcs.NotifyChange();
            }
        }

        public int AdultsCount
        {
            get { return _booking.AdultsCount; }
            set
            {
                _booking.AdultsCount = value;
                _pcs.NotifyChange();
            }
        }

        public int BabiesCount
        {
            get { return _booking.BabiesCount; }
            set
            {
                _booking.BabiesCount = value;
                _pcs.NotifyChange();
            }
        }

        public BookingState State
        {
            get { return _booking.State; }
        }

        public DiscountEntity OptionDiscount
        {
            get
            {
                if(_optionDiscountEntity == null)
                {
                    _optionDiscountEntity = new DiscountEntity(_booking.OptionDiscount);
                }
                return _optionDiscountEntity;
            }
        }

        public OptionChoiceEntity DiscountedOptionChoice
        {
            get { return _discountedOptionChoiceEntity; }
        }

        public double OptionsTotal
        {
            get { return _booking.OptionsTotal; }
        }

        public double RoomsTotal
        {

            get { return _booking.RoomsTotal; }
        }

        public static double TvaVal
        {
            get { return Tva.Value; }
        }


        string IDataErrorInfo.Error
        {
            get { return ((IDataErrorInfo)_booking).Error; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ((IDataErrorInfo)_booking)[columnName]; }
        }

        public BookingEntity(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _booking = booking;
            _clientEntity = new ClientEntity(_booking.Client);
            _paymentEntity = new PaymentEntity(booking);
            _datesEntity = new DateRangeEntity(_booking.Dates);
            _discountedOptionChoiceEntity = new OptionChoiceEntity(_booking, _booking.DiscountedOptionChoice);
            _optionDiscountEntity = new DiscountEntity(_booking.OptionDiscount);
        }
    }
}
