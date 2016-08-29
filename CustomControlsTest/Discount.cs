using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ResotelApp.Models
{
    public class Discount : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;
        
        public int Id { get; set; }
        [Required]
        public double ReduceByPercent { get; set; }
        public double PackPrice { get; set; }
        public int PackQuantity { get; set; }
        public DateRange Validity { get; set; }

      
        

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
                { nameof(PackQuantity), _validatePackQuantity},
                { nameof(Validity), _validateValidity }
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

        private string _validatePackQuantity()
        {
            string error = null;
            if(PackQuantity <= 0)
            {
                error = string.Format("La promotion {0} est invalide car la quantité doit être strictement positive.", Id);
            }
            return error;
        }

        private string _validatePackPrice()
        {
            string error = null;
            if(PackPrice < 0)
            {
                error = string.Format("La promotion {0} est invalide car le prix du pack doit être positif", Id);
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


        /// <summary>Validates a discount</summary>
        /// <returns>true if the discount is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool reduceByPercentValidates = _validateReduceByPercent() == null;
            bool packQuantityValidates = _validatePackQuantity() == null;
            bool validityValidates = _validateValidity() == null;
            bool packPriceValidates = _validatePackPrice() == null;

            return reduceByPercentValidates && packQuantityValidates && validityValidates
                && packPriceValidates;
        }
    }
}