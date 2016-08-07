using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class RoomChoiceEntity : IEntity, INotifyPropertyChanged, ICloneable
    {
        private PropertyChangeSupport _pcs;
        private int _count;
        private string _imageFullPath;
        private BedKind _bedKind;

        public int Count
        {
            get { return _count; }

            set
            {
                _count = value;
                _pcs.NotifyChange();
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

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }



        public RoomChoiceEntity(string imageFullPath, BedKind bedKind, int count=0)
        {
            _pcs = new PropertyChangeSupport(this);
            _imageFullPath = imageFullPath;
            _bedKind = bedKind;
            _count = count;
        }

        public object Clone()
        {
            RoomChoiceEntity roomChoice = new RoomChoiceEntity(_imageFullPath, _bedKind, _count);
            return roomChoice;
        }
    }
}