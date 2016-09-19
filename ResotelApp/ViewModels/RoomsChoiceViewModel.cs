using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.Utils;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class RoomsChoiceViewModel : INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private ObservableCollection<RoomChoiceEntity> _availableRoomChoiceEntities;
        private ICollectionView _availableRoomChoiceEntitiesView;
        private ICollectionViewSource _availableRoomChoiceEntitiesSource;
        private List<Room> _availableRooms;
        private List<Room> _filteredRooms;
        private Dictionary<RoomKind, int> _availableRoomCounts;

        private DelegateCommandAsync<Booking> _assignRoomsCommand;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public ICollectionView AvailableRoomChoiceEntitiesView
        {
            get { return _availableRoomChoiceEntitiesView; }
            set
            {
                _availableRoomChoiceEntitiesView = value;
                _pcs.NotifyChange();
            }
        }

        public List<Room> AvailableRooms
        {
            get { return _availableRooms; }
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

        private RoomsChoiceViewModel()
        {
            _pcs = new PropertyChangeSupport(this);
            _availableRoomChoiceEntities = new ObservableCollection<RoomChoiceEntity>();
            _availableRoomChoiceEntitiesSource = CollectionViewProvider.Provider(_availableRoomChoiceEntities);
            _availableRoomChoiceEntitiesView = _availableRoomChoiceEntitiesSource.View;
            _filteredRooms = new List<Room>();
            _availableRoomCounts = new Dictionary<RoomKind, int>();
        }

        public static async Task<RoomsChoiceViewModel> CreateAsync(DateRange dates)
        {
            RoomsChoiceViewModel newInstance = new RoomsChoiceViewModel();
            newInstance._availableRooms = await RoomRepository.GetAvailablesBetweenAsync(dates);

            int i = 0;
            foreach (Room room in newInstance._availableRooms)
            {
                if (!newInstance._availableRoomCounts.ContainsKey(room.Kind))
                {
                    newInstance._availableRoomCounts.Add(room.Kind, 1);
                    RoomChoiceEntity roomChoice = new RoomChoiceEntity(room.Kind, 0, 0);
                    newInstance._availableRoomChoiceEntities.Add(roomChoice);
                    i++;
                }
                else
                {
                    newInstance._availableRoomCounts[room.Kind]++;
                }
            }

            foreach (RoomChoiceEntity roomChoice in newInstance._availableRoomChoiceEntities)
            {
                roomChoice.MaxAvailable = newInstance._availableRoomCounts[roomChoice.RoomKind];
            }

            return newInstance;
        }

        public void Update(OptionChoiceEntity optChoiceEntity)
        {
            Predicate<Room> missChoosenOpt = room => room.Options.FindIndex(opt => opt.Id == optChoiceEntity.OptionChoice.Option.Id) == -1;

            if (optChoiceEntity.Taken)
            {
                for(int i=_availableRooms.Count -1; i>=0; i--)
                {
                    if(missChoosenOpt(_availableRooms[i]))
                    {
                        _filteredRooms.Add(_availableRooms[i]);
                        _availableRoomCounts[_availableRooms[i].Kind]--;
                        _availableRooms.RemoveAt(i);
                    }
                }


                for (int i = _availableRoomChoiceEntities.Count - 1; i >= 0; i--)
                {
                    RoomKind roomKind = _availableRoomChoiceEntities[i].RoomKind;
                    if (_availableRoomCounts[roomKind] == 0)
                    {
                        _availableRoomChoiceEntities.RemoveAt(i);
                    }
                    else
                    {
                        _availableRoomChoiceEntities[i].MaxAvailable = _availableRoomCounts[roomKind];
                    }
                }
            } else
            {
                HashSet<RoomKind> newlyAvailableRoomKinds = new HashSet<RoomKind>();
                HashSet<RoomKind> extraRoomAvailableKinds = new HashSet<RoomKind>();

                for (int i=_filteredRooms.Count -1; i>=0; i--)
                {
                    if(missChoosenOpt(_filteredRooms[i]))
                    {
                        _availableRooms.Add(_filteredRooms[i]);
                        _availableRoomCounts[_filteredRooms[i].Kind]++;
                        if (_availableRoomCounts[_filteredRooms[i].Kind] == 1)
                        {
                            newlyAvailableRoomKinds.Add(_filteredRooms[i].Kind);
                        } else
                        {
                            extraRoomAvailableKinds.Add(_filteredRooms[i].Kind);
                        }
                        _filteredRooms.RemoveAt(i);
                    }
                }

                foreach(RoomKind kind in newlyAvailableRoomKinds)
                {
                    RoomChoiceEntity roomChoiceEntity = new Entities.RoomChoiceEntity(kind, _availableRoomCounts[kind], 0);
                    _availableRoomChoiceEntities.Add(roomChoiceEntity);
                }

                foreach(RoomKind kind in extraRoomAvailableKinds)
                {
                    foreach(RoomChoiceEntity roomChoiceEntity in _availableRoomChoiceEntities)
                    {
                        if(roomChoiceEntity.RoomKind.Equals(kind))
                        {
                            roomChoiceEntity.MaxAvailable = _availableRoomCounts[kind];
                            break;
                        }
                    }
                }
            }
        }

        private async Task _assignRooms(Booking booking)
        {
            try
            {
                List<Option> choosenOptions = booking.OptionChoices.ConvertAll(optChoice => optChoice.Option);
                List<Room> matchingRooms = await RoomRepository.GetMatchingRoomsBetween(choosenOptions, booking.Dates);
                booking.Rooms.Clear();
                foreach (RoomChoiceEntity roomChoice in _availableRoomChoiceEntities)
                {
                    if (roomChoice.Count > 0)
                    {
                        IList<Room> rooms = _findRooms(matchingRooms, roomChoice.BedKind, roomChoice.Count);
                        if (rooms.Count > 0)
                        {
                            matchingRooms.RemoveAll(matchingRoom => booking.Rooms.FindIndex(room => room.Id == matchingRoom.Id) != -1);
                            booking.Rooms.AddRange(rooms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Log(ex);
            }
        }

        private IList<Room> _findRooms(IEnumerable<Room> availableRooms, BedKind bedKind, int count)
        {
            List<Room> rooms = new List<Room>(count);

            foreach(Room room in availableRooms)
            {
                if(room.BedKind == bedKind && rooms.Count < count)
                {
                    rooms.Add(room);
                }
                else if(rooms.Count == count)
                {
                    break;
                }
            }

            return rooms;
        }
    }
}
