using ResotelApp.Models;
using System.Collections.Generic;

namespace ResotelApp.ViewModels.Entities
{
    class BookedRoomEntity : IEntity
    {
        private Room _room;
        private DateRange _takenDates;
        private List<AppliedPack> _appliedDiscounts;
        private List<OptionChoiceEntity> _optionChoiceEntities;

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

        public IList<OptionChoiceEntity> OptionChoiceEntities
        {
            get { return _optionChoiceEntities; }
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

            List<OptionChoiceEntity> optChoiceEntities =
                booking.OptionChoices.ConvertAll(optChoice => new OptionChoiceEntity(optChoice));
            _optionChoiceEntities = new List<OptionChoiceEntity>();
            _optionChoiceEntities.AddRange(optChoiceEntities);
        }
    }
}
