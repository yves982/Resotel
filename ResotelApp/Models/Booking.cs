using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;

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
        public int AdultsCount { get; set; }
        public int BabiesCount { get; set; }
        
        public DateTime? TerminatedDate { get; set; }
        public BookingState State
        {
            get
            {
                BookingState state = BookingState.Validated;
                if(Payment != null && Payment.Ammount > 0.0d && TerminatedDate == null)
                {
                    state = BookingState.Paid;
                }
                else if (TerminatedDate.HasValue && Dates.Start.Subtract(TerminatedDate.Value).TotalDays >= 2d )
                {
                    state = BookingState.FullyCancelled;
                } else if(TerminatedDate.HasValue)
                {
                    state = BookingState.Cancelled;
                }
                return state;
            }
        }
        public Payment Payment { get; set; }

        public static double Tva
        {
            get;set;
        }

        public Discount OptionDiscount
        {
            get
            {
                if (_optionDiscount == null)
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
                if (_discountedOptionChoice == null)
                {
                    _computeOptionDiscount();
                }
                return _discountedOptionChoice;
            }
        }

        public double OptionsTotal
        {
            get
            {
                double optionsTotal = 0;
                foreach (OptionChoice optChoice in OptionChoices)
                {
                    optionsTotal += optChoice.ActualPrice;
                }
                return optionsTotal;
            }
        }

        public double RoomsTotal
        {

            get
            {
                double roomsTotal = 0;
                foreach (AppliedPack appliedPack in RoomPacks)
                {
                    roomsTotal += appliedPack.Price * appliedPack.Count;
                }
                return roomsTotal;
            }
        }

        public double TotalHT
        {
            get { return RoomsTotal + OptionsTotal; }
        }

        public double Total
        {
            get { return (RoomsTotal + OptionsTotal) * (1d + Tva / 100d); }
        }

        public List<AppliedPack> RoomPacks
        {
            get
            {
                _roomDiscounts.Clear();
                foreach (Room room in Rooms)
                {
                    List<Pack> orderedPacks = new List<Pack>(room.AvailablePacks);
                    orderedPacks.Sort((firstDiscount, secondDiscount) =>
                    secondDiscount.Quantity.CompareTo(firstDiscount.Quantity));
                    int leftDays = Dates.Days;
                    foreach (Pack pack in orderedPacks)
                    {
                        int quantity = leftDays / pack.Quantity;
                        leftDays = leftDays % pack.Quantity;
                        if (quantity > 0)
                        {
                            AppliedPack appliedDiscount = new AppliedPack { Count = quantity, RoomPack = pack, Room = room };
                            _roomDiscounts.Add(appliedDiscount);
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

                foreach (KeyValuePair<string, Func<string>> propValidationFnKvp in _propertiesValidations)
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
                if (_propertiesValidations.ContainsKey(columnName))
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
            }
            else
            {
                error = string.Format("le client de la réservation: {0} ne peut être null", Id);
            }
            return error;
        }

        private string _validatePayment()
        {
            string error = null;
            if (Payment != null)
            {
                error = ((IDataErrorInfo)Payment).Error;
            }
            return error;
        }

        private string _validateOptions()
        {
            List<OptionChoice> invalidOptions = new List<OptionChoice>(OptionChoices);
            invalidOptions.RemoveAll(opt => opt == null || string.IsNullOrEmpty(((IDataErrorInfo)opt).Error));
            string errors = string.Join(";",
                invalidOptions.ConvertAll<string>(opt => ((IDataErrorInfo)opt).Error)
            );

            if (errors.Length == 0)
            {
                errors = null;
            }
            return errors;
        }

        private string _validateRooms()
        {
            List<Room> invalidRooms = new List<Room>(Rooms);
            invalidRooms.RemoveAll(room => room == null || string.IsNullOrEmpty(((IDataErrorInfo)room).Error));
            string errors = string.Join(";",
                invalidRooms.ConvertAll<string>(room => ((IDataErrorInfo)room).Error)
            );

            if (errors.Length == 0)
            {
                errors = null;
            }
            return errors;
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
            Payment = new Payment();
            _roomDiscounts = new List<AppliedPack>();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Client), _validateClient },
                { nameof(Payment), _validatePayment },
                { nameof(OptionChoices), _validateOptions },
                { nameof(Rooms), _validateRooms },
                { nameof(Dates), _validateDates },
                { nameof(AdultsCount), _validateAdultsCount },
                { nameof(BabiesCount), _validateBabiesCount }
            };
        }

        public static Expression<Func<Booking, bool>> IsFullyCancelled()
        {
            return booking => booking.TerminatedDate.HasValue
            && SqlFunctions.DateDiff("day", booking.TerminatedDate, booking.Dates.Start) >= 2d;
        }

        public static Expression<Func<Booking, bool>> IsNotFullyCancelled()
        {
            return booking => !booking.TerminatedDate.HasValue
            || SqlFunctions.DateDiff("day", booking.TerminatedDate, booking.Dates.Start) < 2d;
        }

        public static Expression<Func<Booking, bool>> IsCancelled()
        {
            return booking => booking.TerminatedDate.HasValue
            && SqlFunctions.DateDiff("day", booking.TerminatedDate, booking.Dates.Start) < 2d;
        }


        public static Expression<Func<Booking, bool>> IsNotCancelled()
        {
            return booking => !booking.TerminatedDate.HasValue
            || SqlFunctions.DateDiff("day", booking.TerminatedDate, booking.Dates.Start) >= 2d;
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
            bool paymentValidates = _validatePayment() == null;
            bool optionsValidates = _validateOptions() == null;
            bool roomValidates = _validateRooms() == null;
            bool datesValidates = _validateDates() == null;
            bool adultCountValidates = _validateAdultsCount() == null;
            bool babiesCountValidates = _validateBabiesCount() == null;

            return clientValidates && optionsValidates && paymentValidates
                && roomValidates && datesValidates && adultCountValidates 
                && babiesCountValidates;
        }
    }
}
