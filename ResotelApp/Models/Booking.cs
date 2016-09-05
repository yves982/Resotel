using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ResotelApp.Models
{
    public class Booking : IValidable, IDataErrorInfo
    {
        private Discount _optionDiscount;
        private OptionChoice _discountedOptionChoice;
        private List<AppliedPack> _roomDiscounts;

        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        [Required]
        public Client Client { get; set; }
        public List<OptionChoice> OptionChoices { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Room> Rooms { get; set; }
        public DateRange Dates { get; set; }
        public int AdultsCount { get;set; }
        public int BabiesCount { get; set; }
        public BookingState State { get; set; }

        public Discount OptionDiscount
        {
            get
            {
                if(_optionDiscount == null)
                {
                    _computeOptionDiscount();
                }
                return _optionDiscount;
            }
        }

        public OptionChoice DiscountedOptionChoice
        {
            get
            {
                if(_discountedOptionChoice == null)
                {
                    _computeOptionDiscount();
                }
                return _discountedOptionChoice;
            }
        }

        public List<AppliedPack> RoomPacks
        {
            get
            {
                if(_roomDiscounts.Count == 0 && Rooms.Count > 0)
                {
                    _roomDiscounts.Clear();
                    foreach(Room room in Rooms)
                    {
                        List<Pack> orderedPacks = new List<Pack>(room.AvailablePacks);
                        orderedPacks.Sort((firstDiscount, secondDiscount) =>
                        secondDiscount.Quantity.CompareTo(firstDiscount.Quantity));
                        int leftDays = Dates.Days;
                        foreach(Pack pack in orderedPacks)
                        {
                            int quantity = leftDays / pack.Quantity;
                            leftDays = leftDays % pack.Quantity;
                            if(quantity > 0)
                            {
                                AppliedPack appliedDiscount = new AppliedPack { Count=quantity, RoomPack=pack, Room = room };
                                _roomDiscounts.Add(appliedDiscount);
                            }
                        }
                    }
                }
                return _roomDiscounts;
            }
            set
            {
                _roomDiscounts = value;
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                string error = null;
                StringBuilder stringBuilder = new StringBuilder();

                foreach(KeyValuePair<string, Func<string>> propValidationFnKvp in _propertiesValidations)
                {
                    string propError = propValidationFnKvp.Value();
                    if (propError != null)
                    {
                        stringBuilder.Append(propError + ";");
                    }
                }
                
                if (stringBuilder.Length > 0)
                {
                    error = stringBuilder.ToString();
                }
                return error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string error = null;
                if(_propertiesValidations.ContainsKey(columnName))
                {
                    error = _propertiesValidations[columnName]();
                }
                return error;
            }
        }

        private void _computeOptionDiscount()
        {
            List<OptionChoice> sortedOptionChoices = new List<OptionChoice>(OptionChoices);
            sortedOptionChoices.Sort((firstOptChoice, secondOptChoice) =>
                secondOptChoice.DiscountedAmmount.CompareTo(firstOptChoice.DiscountedAmmount)
            );
            if (sortedOptionChoices.Count > 0)
            {
                _optionDiscount = sortedOptionChoices[0].Option.CurrentDiscount;
                _discountedOptionChoice = sortedOptionChoices[0];
            }
        }

        private string _validateClient()
        {
            string error = null;
            if (Client != null)
            {
                error = ((IDataErrorInfo)Client).Error;
            } else
            {
                error = string.Format("le client de la réservation: {0} ne peut être null", Id);
            }
            return error;
        }

        private string _validateOptions()
        {
            List<OptionChoice> invalidOptions = new List<OptionChoice>(OptionChoices);
            invalidOptions.RemoveAll(opt => opt == null || string.IsNullOrEmpty(((IDataErrorInfo)opt).Error));
            return string.Join(";", 
                invalidOptions.ConvertAll<string>( opt => ((IDataErrorInfo)opt).Error )
            );
        }

        private string _validateRooms()
        {
            List<Room> invalidRooms = new List<Room>(Rooms);
            invalidRooms.RemoveAll(room => room == null || string.IsNullOrEmpty(((IDataErrorInfo)room).Error));
            return string.Join(";",
                invalidRooms.ConvertAll<string>(room => ((IDataErrorInfo)room).Error)
            );
        }

        private string _validateDates()
        {
            return ((IDataErrorInfo)Dates).Error;
        }

        private string _validateAdultsCount()
        {
            string error = null;
            if (AdultsCount <= 0)
            {
                error = string.Format("Le nombre d'adulte de la réservation: {0} doit être strictement positif", Id);
            }
            return error;
        }

        private string _validateBabiesCount()
        {
            string error = null;
            if (BabiesCount < 0 || BabiesCount > 1)
            {
                error = string.Format("Le nombre de bébés de la réservation: {0} doit être entre 0 et 1", Id);
            }
            return error;
        }

        public Booking()
        {
            Client = new Client();
            OptionChoices = new List<OptionChoice>();
            Rooms = new List<Room>();
            Dates = new DateRange();
            _roomDiscounts = new List<AppliedPack>();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Client), _validateClient },
                { nameof(OptionChoices), _validateOptions },
                { nameof(Rooms), _validateRooms },
                { nameof(Dates), _validateDates },
                { nameof(AdultsCount), _validateAdultsCount },
                { nameof(BabiesCount), _validateBabiesCount }
            };
        }

        public void AddClient(Client client)
        {
            Client = client;
            client.Bookings.Add(this);
        }

        /// <summary>Validates a Booking</summary>
        /// <returns>true if the booking is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool clientValidates = _validateClient() == null;
            bool optionsValidates = _validateOptions() == null;
            bool roomValidates = _validateRooms() == null;
            bool datesValidates = _validateDates() == null;
            bool adultCountValidates = _validateAdultsCount() == null;
            bool babiesCountValidates = _validateBabiesCount() == null;

            return clientValidates && optionsValidates && roomValidates
                && datesValidates && adultCountValidates && babiesCountValidates;
        }
    }
}
