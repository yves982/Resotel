using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Xps.Packaging;

namespace ResotelApp.ViewModels
{
    class SumUpViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private string _title;
        private LinkedList<INavigableViewModel> _navigation;
        private List<BookedRoomEntity> _bookedRoomEntity;
        private XpsDocument _xpsDoc;

        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }

        public string Title
        {
            get { return _title; }
        }

        public IList<BookedRoomEntity> BookedRoomEntities
        {
            get { return _bookedRoomEntity; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;
        public event EventHandler<string> MessageReceived;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public SumUpViewModel(Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _bookedRoomEntity = new List<BookedRoomEntity>();

            foreach(Room room in booking.Rooms)
            {
                BookedRoomEntity bookedRoomEntity = new BookedRoomEntity(booking, room);
                _bookedRoomEntity.Add(bookedRoomEntity);
            }
        }
    }
}
