using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class RoomChoiceEntity : IEntity, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private int _count;
        private string _imageFullPath;
        private BedKind _bedKind;
        private RoomKind _roomKind;
        private int _capacity;
        private int _maxAvailable;
        private string _maxTooltip;

        public int Count
        {
            get { return _count; }

            set
            {
                _count = value;
                _pcs.NotifyChange();
            }
        }

        public int MaxAvailable
        {
            get { return _maxAvailable; }
            set
            {
                _maxAvailable = value;
                _pcs.NotifyChange();
            }
        }

        public string MaxTooltip
        {
            get
            {
                if(_maxTooltip == null)
                {
                    _maxTooltip = string.Format("Le maximum possible est de {0}", _maxAvailable);
                }
                return _maxTooltip;
            }
        }

        public string ImageFullPath
        {
            get { return _imageFullPath; }

            set
            {
                _imageFullPath = value;
                _pcs.NotifyChange();
            }
        }

        public BedKind BedKind
        {
            get { return _bedKind; }

            set
            {
                _bedKind = value;
                _pcs.NotifyChange();
            }
        }

        public RoomKind RoomKind
        {
            get { return _roomKind; }
            set
            {
                _roomKind = value;
                _pcs.NotifyChange();
            }
        }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                _capacity = value;
                _pcs.NotifyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }



        public RoomChoiceEntity(RoomKind kind, int count=0)
        {
            _pcs = new PropertyChangeSupport(this);
            _imageFullPath = string.Format("/Resources/room_{0}.png", kind.ToString());
            _bedKind = kind.ToBedKind();
            _roomKind = kind;
            _count = count;
        }
    }
}