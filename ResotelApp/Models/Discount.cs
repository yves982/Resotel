using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ResotelApp.Models
{
    public class Discount : IValidable, IDataErrorInfo
    {
        private OptionChoice _option;
        private Room _room;
        private Dictionary<string, Func<string>> _propertiesValidations;
        
        public int Id { get; set; }
        [Required]
        public double ReduceByPercent { get; set; }
        public int ApplicableQuantity { get; set; }
        public DateRange Validity { get; set; }

      
        public OptionChoice Option
        {
            get { return _option; }
            set
            {
                if(_room != null)
                {
                    throw new InvalidOperationException("On ne peut affecter une option à une promotion de chambre");
                }
                _option = value;
            }
        }

        public Room Room
        {
            get { return _room; }
            set
            {
                if(_option != null)
                {
                    throw new InvalidOperationException("On ne peut affecter une chambre à une promotion d'option.");
                }
                _room = value;
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
                if(_propertiesValidations.ContainsKey(columnName))
                {
                    error = _propertiesValidations[columnName]();
                }
                return error;
            }
        }

        public Discount()
        {
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(ReduceByPercent), _validateReduceByPercent },
                { nameof(ApplicableQuantity), _validateApplicableQuantity},
                { nameof(Validity), _validateValidity },
                { nameof(Option), _validateOptionAndRoom },
                { nameof(Room), _validateOptionAndRoom }
            };
        }
        
        private string _validateReduceByPercent()
        {
            string error = null;
            if(ReduceByPercent < 0)
            {
                error = string.Format("La promotion {0} est invalide car le pourcentage de réduction doit être positif ou null.", Id);
            }
            return error;
        }

        private string _validateApplicableQuantity()
        {
            string error = null;
            if(ApplicableQuantity <= 0)
            {
                error = string.Format("La promotion {0} est invalide car la quantité doit être strictement positive.", Id);
            }
            return error;
        }

        private string _validateValidity()
        {
            string error = null;
            string validityError = Validity == null ? null : ((IDataErrorInfo)Validity).Error;
            if (validityError != null)
            {
                error = string.Format("La validité de la promotion {0} est invalide car {1}", Id, validityError);
            }
            return error;
        }

        private string _validateOptionAndRoom()
        {
            string error = null;
            if(_option == null && _room == null)
            {
                error = string.Format("La promotion {0} n'est pas valide car elle n'a pas d'objet (ni chambre ni option).", Id);
            }

            return error;
        }


        /// <summary>Validates a discount</summary>
        /// <returns>true if the discount is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool reduceByPercentValidates = _validateReduceByPercent() == null;
            bool applicableQuantityValidates = _validateApplicableQuantity() == null;
            bool validityValidates = _validateValidity() == null;
            bool optionAndRoomValidates = _validateOptionAndRoom() == null;

            return reduceByPercentValidates && applicableQuantityValidates && validityValidates
                && optionAndRoomValidates;
        }
    }
}