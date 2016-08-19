using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResotelApp.Models
{
    public class Booking : IValidable, IDataErrorInfo
    {
        private Discount _roomDiscount;
        private Discount _optionDiscount;
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

        [NotMapped]
        public Discount RoomDiscount
        {
            get { return _roomDiscount; }
            set
            {
                string errMessage = "";
                if(value.Room == null)
                {
                    errMessage = string.Format("La promotion de chambre de la réservation {0} doit contenir une chambre.", Id);
                    throw new InvalidOperationException(errMessage);
                } else if(!Rooms.Contains(value.Room))
                {
                    errMessage = string.Format("Cette promotion de chambre concerne une chambre ne faisant pas partie de la réservation {0}", Id);
                    throw new InvalidOperationException(errMessage);
                }
                _roomDiscount = value;
            }
        }

        public Discount OptionDiscount
        {
            get { return _optionDiscount; }
            set
            {
                string errMessage = "";
                if(value.Option == null)
                {
                    errMessage = string.Format("La promotion d'option de la réservation {0} doit contenir une option.", Id);
                    throw new InvalidOperationException(errMessage);
                } else if (OptionChoices.FindIndex( (Predicate<OptionChoice>)(optChoice => optChoice.Option.Id == value.Option.Id)) == -1)
                {
                    errMessage = string.Format("Cette promotion d'option concerne une option ne faisant pas partie de la réservation {0}"
                        , Id);
                    throw new InvalidOperationException(errMessage);
                }
                _optionDiscount = value;
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

        private string _validateRoomDiscount()
        {
            string error = null;
            if (_roomDiscount != null && _roomDiscount.Room == null)
            {
                error = string.Format("La promotion de chambre {1} de la réservation {0} n'est pas valide, car sa chambre n'est pas renseignée", Id, _roomDiscount.Id);
            } else if (_roomDiscount != null && !Rooms.Contains(_roomDiscount.Room))
            {
                error = string.Format("La promotion de chambre {1} ne concerne pas une chambre de la réservation {0}", Id, _roomDiscount.Id);
            }
            return error;
        }

        private string _validateOptionDiscount()
        {
            string error = null;
            if (_optionDiscount != null && _optionDiscount.Option == null)
            {
                error = string.Format("La promotion d'option {1} de la réservation {0} n'est pas valide, car son option n'est pas renseignée", Id, _optionDiscount.Id);
            }
            else if (_optionDiscount != null &&
                OptionChoices.FindIndex( (Predicate<OptionChoice>)(optChoice => optChoice.Option.Id == _optionDiscount.Option.Id)) == -1)
            {
                error = string.Format("La promotion d'option {1} ne concerne pas une option de la réservation {0}", Id, _optionDiscount.Id);
            }
            return error;
        }

        public Booking()
        {
            Client = new Client();
            OptionChoices = new List<OptionChoice>();
            Rooms = new List<Room>();
            Dates = new DateRange();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Client), _validateClient },
                { nameof(OptionChoices), _validateOptions },
                { nameof(Rooms), _validateRooms },
                { nameof(Dates), _validateDates },
                { nameof(AdultsCount), _validateAdultsCount },
                { nameof(BabiesCount), _validateBabiesCount },
                { nameof(RoomDiscount), _validateRoomDiscount },
                { nameof(OptionDiscount), _validateOptionDiscount }
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
            bool roomDiscountValidates = _validateRoomDiscount() == null;
            bool optionDiscountValidates = _validateOptionDiscount() == null;

            return clientValidates && optionsValidates && roomValidates
                && datesValidates && adultCountValidates && babiesCountValidates
                && roomDiscountValidates && optionDiscountValidates;
        }
    }
}
