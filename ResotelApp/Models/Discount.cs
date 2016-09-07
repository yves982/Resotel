using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResotelApp.Models
{
    /// <summary> 
    ///     This class is intended to hold discount and pack informations.
    ///     Packs only have PackPrice and PackQuantity while Discounts have reduceByPercent and optionally Validity
    /// </summary>
    public class Discount : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        [Required]
        public double ReduceByPercent { get; set; }
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
            
            bool validityValidates = _validateValidity() == null;
            

            return reduceByPercentValidates && validityValidates;
        }
    }
}