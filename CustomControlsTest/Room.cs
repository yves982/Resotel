﻿using ResotelApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ResotelApp.Models
{
    public class Room : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;
        private RoomKind? _kind;

        public int Id { get; set; }
        [Required]
        public int Stage { get; set; }
        [Required]
        [DefaultValue(1)]
        public int Capacity { get; set; }
        public List<Option> Options { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Discount> AvailablePacks { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsCleaned { get; set; }
        public BedKind BedKind { get; set; }
        public double Price { get; set; }

        [NotMapped]
        public RoomKind Kind
        {
            get
            {
                if (!_kind.HasValue)
                {
                    switch (Capacity)
                    {
                        case 1:
                            _kind = RoomKind.Simple;
                            break;
                        case 2:
                            _kind = BedKind.Equals(BedKind.DoubleWithBaby) ? RoomKind.DoubleWithBaby : RoomKind.Double;
                            break;
                        case 3:
                            _kind = RoomKind.Three;
                            break;
                        case 4:
                            _kind = RoomKind.Four;
                            break;
                        case 6:
                            _kind = RoomKind.Six;
                            break;
                        default:
                            _kind = RoomKind.Simple;
                            break;
                    }
                }
                return _kind.Value;
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

        public static Expression<Func<Room,bool>> _hasNoBooking()
        {
            return room => room.Bookings.Count == 0;
        }

        private static Expression<Func<Room, bool>> _bookingsStartedAfter(DateTime end)
        {
            return room => !room.Bookings.Any(booking => SqlFunctions.DateDiff("day",booking.Dates.Start, end) > 0);
        } 

        private static Expression<Func<Room, bool>> _bookingsEndedBefore(DateTime start)
        {
            return room => !room.Bookings.Any(booking => SqlFunctions.DateDiff("day", booking.Dates.End, start) < 0);
        }

        public static Expression<Func<Room, bool>> NotCurrentlyBooked()
        {
            return _hasNoBooking()
                .Or(_bookingsStartedAfter(DateTime.Now.AddDays(1.0)))
                .Or(_bookingsEndedBefore(DateTime.Now));
        }

        public static Expression<Func<Room, bool>> NotBookedDuring(DateRange dateRange)
        {
            return _hasNoBooking()
                .Or(_bookingsStartedAfter(dateRange.End))
                .Or(_bookingsEndedBefore(dateRange.Start));
        }

        public static Expression<Func<Room, bool>> WithBedKind(BedKind bedKind)
        {
            return room => room.BedKind == bedKind;
        }

        public static Expression<Func<Room, bool>> WithOptions(IEnumerable<Option> seekedOptions)
        {
            return room => !seekedOptions.Any(seekedOpt => !room.Options.Contains(seekedOpt));
        }

        

        public Room()
        {
            Options = new List<Option>();
            Bookings = new List<Booking>();
            AvailablePacks = new List<Discount>();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Stage), _validateStage },
                { nameof(Capacity), _validateCapacity },
                { nameof(Options), _validateOptions },
                {nameof(Price), _validatePrice },
                { nameof(AvailablePacks), _validateAvailablePacks }
            };
        }

        private string _validateStage()
        {
            string error = null;
            if(Stage <= 0)
            {
                error = string.Format("La chambre {0} est invalide car son étage doit être strictement positif.", Id);
            }
            return error;
        }

        private string _validateCapacity()
        {
            string error = null;
            if(Capacity <= 0)
            {
                error = string.Format("La chambre {0} est invalide car sa capacité doit être strictement positive", Id);
            }
            return error;
        }

        private string _validateOptions()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<Option> invalidOptions = new List<Option>(Options);
            invalidOptions.RemoveAll(opt => (opt == null ? null : ((IDataErrorInfo)opt).Error) == null);
            string optErrors = string.Join(";", invalidOptions.ConvertAll(opt => ((IDataErrorInfo)opt).Error));
            string error = string.Format("La chambre {0} est invalide car: {1}", Id, optErrors );
            return error;
        }

        private string _validatePrice()
        {
            string error = null;
            if(Price < 0)
            {
                error = string.Format("La chambre {0} est invalide car son prix doit être positif ou null.", Id);
            }
            return error;
        }

        private string _validateAvailablePacks()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<Discount> invalidPacks = new List<Discount>(AvailablePacks);
            invalidPacks.RemoveAll(discount => (discount == null ? null : ((IDataErrorInfo)discount).Error) == null);
            string packsError = string.Join(";", invalidPacks.ConvertAll(discount => ((IDataErrorInfo)discount).Error));
            string error = string.Format("La chambre {0} est invalide car: {1}", Id, packsError);
            return error;
        }

        /// <summary>Validates a Room</summary>
        /// <returns>true if the room is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool stageValidates = _validateStage() == null;
            bool capacityValidates = _validateCapacity() == null;
            bool optionsValidates = _validateOptions() == null;
            bool priceValidates = _validatePrice() == null;
            bool validateAvailablePacks = _validateAvailablePacks() == null;

            return stageValidates && capacityValidates && optionsValidates
                 && priceValidates && validateAvailablePacks;
        }
    }
}
