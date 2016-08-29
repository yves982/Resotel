using ResotelApp.Models;

namespace ResotelApp.ViewModels.Entities
{
    class BookedRoomEntity : IEntity
    {
        private Room _room;
        private Booking _booking;

        public Room Room
        {
            get { return _room; }
        }

        public Booking Booking
        {
            get { return _booking; }
        }

        public BookedRoomEntity(Room room, Booking booking)
        {
            _room = room;
            _booking = booking;
        }
    }
}
