using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.Collections.Generic;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class BookedRoomEntity : IEntity
    {
        private Room _room;
        private DateRange _takenDates;
        private List<AppliedPack> _appliedDiscounts;

        public Room Room
        {
            get { return _room; }
        }

        public DateRange TakenDates
        {
            get { return _takenDates; }

        }

        public IEnumerable<AppliedPack> AppliedDiscounts
        {
            get { return _appliedDiscounts; }
        }

        public BookedRoomEntity(Booking booking, Room room)
        {
            _room = room;
            _takenDates = booking.Dates;
            _appliedDiscounts = new List<AppliedPack>();
            foreach(AppliedPack appliedDiscount in booking.RoomPacks)
            {
                if(appliedDiscount.Room.Id == _room.Id)
                {
                    _appliedDiscounts.Add(appliedDiscount);
                }
            }
        }
    }
}
