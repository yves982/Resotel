using ResotelApp.Models;
using System.Collections.Generic;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// A Collection of AppliedPackEntities with same Room.
    /// </summary>
    class BookedRoomEntity : IEntity
    {
        private Room _room;
        private List<AppliedPackEntity> _appliedPackEntities;

        public Room Room
        {
            get { return _room; }
        }

        public IEnumerable<AppliedPackEntity> AppliedPackEntities
        {
            get { return _appliedPackEntities; }
        }

        public BookedRoomEntity(Booking booking, Room room)
        {
            _room = room;
            _appliedPackEntities = new List<AppliedPackEntity>();
            foreach(AppliedPack appliedPack in booking.RoomPacks)
            {
                if(appliedPack.Room.Id == _room.Id)
                {
                    _appliedPackEntities.Add(new AppliedPackEntity(appliedPack));
                }
            }
        }
    }
}
