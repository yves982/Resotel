using ResotelApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ResotelApp.Models
{
    /// <summary>An option is a thing which may add something to a Booking's price. It's not mandatory to take any, and some are even free.</summary>
    public class Option : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        [Required]
        public string Label { get; set; }
        public List<Room> Rooms { get; set; }
        [Required]
        public double BasePrice { get; set; }

        public List<Discount> Discounts { get; set; }

        public bool HasChooseableDates { get; set; }

        [NotMapped]
        public Discount CurrentDiscount
        {
            get
            {
                Discount currentDiscount = null;
                if (Discounts != null)
                {
                    foreach (Discount discount in Discounts)
                    {
                        if (discount.Validity == null || discount.Validity.Contains(DateTime.Now.Date))
                        {
                            currentDiscount = discount;
                            break;
                        }
                    }
                }

                return currentDiscount;
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

        public Option()
        {
            Rooms = new List<Room>();
            Discounts = new List<Discount>();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Label), _validateLabel },
                { nameof(BasePrice), _validateBasePrice }
            };
        }

        private string _validateLabel()
        {
            string error = null;
            if(Label == null || Label.Length == 0)
            {
                error = string.Format("L'option {0} est invalide car un Label est requis.", Id);
            } else if (Label.Length > 50)
            {
                error = string.Format("L'option {0} est invalide car le Label doit faire au plus 50 caractères.", Id);
            }
            return error;
        }

        private string _validateBasePrice()
        {
            string error = null;
            if(BasePrice < 0)
            {
                error = string.Format("L'option {0} est invalide car son BasePrice doit être positif ou null.", Id);
            }
            return error;
        }

        private static Expression<Func<Option,bool>> _noBookedRoomDuring(DateRange dateRange)
        {
            return opt => opt.Rooms.AsQueryable().Any(Room.NotBookedDuring(dateRange));
        }

        /// <summary>
        /// Indicated wether some rooms are left with this Option, for the requested date
        /// </summary>
        /// <param name="dateRange">requested date</param>
        /// <returns>true if some available rooms with this one are found</returns>
        public static Expression<Func<Option,bool>> IsAvailableBetween(DateRange dateRange)
        {
            Expression<Func<Option, bool>> noRooms = opt => opt.Rooms.Count == 0;
            return noRooms
            .Or(_noBookedRoomDuring(dateRange));
        }

        /// <summary>Validates an Option</summary>
        /// <returns>true if the option is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool labelValidates = _validateLabel() == null;
            bool basePriceValidates = _validateBasePrice() == null;

            return labelValidates && basePriceValidates;
        }
    }
}