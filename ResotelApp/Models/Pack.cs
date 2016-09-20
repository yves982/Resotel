using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.Models
{
    /// <summary>A Pack is the billing side of an AppliedPack, 
    /// it does not bear any information on the RoomKind or Room it'll be applied to, only financial datas</summary>
    public class Pack : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

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

        private string _validateQuantity()
        {
            string error = null;
            if (Quantity <= 0)
            {
                error = string.Format("Le pack {0} est invalide car la quantité doit être strictement positive.", Id);
            }
            return error;
        }

        private string _validatePrice()
        {
            string error = null;
            if (Price < 0)
            {
                error = string.Format("Le pack {0} est invalide car le prix du pack doit être positif", Id);
            }
            return error;
        }


        public Pack()
        {
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Price), _validatePrice },
                { nameof(Quantity), _validateQuantity}
            };
        }

        /// <summary>Validates a pack</summary>
        /// <returns>true if the pack is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool quantityValidates = _validateQuantity() == null;
            bool priceValidates = _validatePrice() == null;

            return quantityValidates && priceValidates;
        }
    }
}
