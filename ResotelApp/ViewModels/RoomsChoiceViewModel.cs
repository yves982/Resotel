using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class RoomsChoiceViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ObservableCollection<RoomChoiceEntity> _availableRoomsChoices;
        private ICollectionView _availableRoomsChoicesView;

        private DelegateCommandAsync<Booking> _assignRoomsCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public ICollectionView AvailableRoomsChoicesView
        {
            get { return _availableRoomsChoicesView; }
            set
            {
                _availableRoomsChoicesView = value;
                _pcs.NotifyChange();
            }
        }

        public DelegateCommandAsync<Booking> AssignRoomsCommand
        {
            get
            {
                if(_assignRoomsCommand == null)
                {
                    _assignRoomsCommand = new DelegateCommandAsync<Booking>(_assignRooms);
                }
                return _assignRoomsCommand;
            }
        }

        public RoomsChoiceViewModel(IEnumerable<RoomChoiceEntity> roomChoices)
        {
            _pcs = new PropertyChangeSupport(this);
            _availableRoomsChoices = new ObservableCollection<RoomChoiceEntity>(roomChoices);
            _availableRoomsChoicesView = CollectionViewProvider.Provider(_availableRoomsChoices);
        }

        private async Task _assignRooms(Booking booking)
        {
            List<Option> choosenOptions = booking.OptionChoices.ConvertAll(optChoice => optChoice.Option);
            List<Room> matchingRooms = await RoomRepository.GetMatchingRoomsBetween(choosenOptions, booking.Dates);
            foreach(RoomChoiceEntity roomChoice in _availableRoomsChoices)
            {
                if(roomChoice.Count>0)
                {
                    Room[] rooms = _findRooms(matchingRooms, roomChoice.BedKind, roomChoice.Count);
                    booking.Rooms.AddRange(rooms);
                }
            }
        }

        private Room[] _findRooms(IEnumerable<Room> availableRooms, BedKind bedKind, int count)
        {
            Room[] rooms = new Room[count];
            int assignedCnt = 0;
            foreach(Room room in availableRooms)
            {
                if(room.BedKind == bedKind && assignedCnt < count)
                {
                    rooms[assignedCnt] = room;
                    assignedCnt++;
                }
                if(assignedCnt == count)
                {
                    break;
                }
            }
            return rooms;
        }
    }
}
