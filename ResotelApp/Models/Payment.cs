using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.Models
{
    /// <summary> A Payment with a mode, a date and an ammount of money </summary>
    public class Payment : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public DateTime? Date { get; set; }
        public double Ammount { get; set; }
        public PaymentMode Mode { get; set; }

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

        private string _validateAmmount()
        {
            string error = null;
            if(Ammount <= 0)
            {
                error = "Le montant doit être strictement positif";
            }
            return error;
        }

        public Payment()
        {
            _propertiesValidations = new Dictionary<string, Func<string>>
            {
                { nameof(Ammount), _validateAmmount }
            };
        }

        public bool Validate()
        {
            bool validatesAmmount = _validateAmmount() == null;
            return validatesAmmount;
        }
    }
}
