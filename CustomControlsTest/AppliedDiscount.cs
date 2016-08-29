using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ResotelApp.Models
{
    public class AppliedDiscount : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        public Discount Discount { get; set; }
        public Room Room { get; set; }
        public int Count { get; set; }

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

        public AppliedDiscount()
        {
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Discount), _validateDiscount },
                { nameof(Count), _validateCount },
                { nameof(Room), _validateRoom }
            };
        }

        private string _validateDiscount()
        {
            string error = null;
            if (Discount == null)
            {
                error = string.Format("L'application de promotion {0} n'est pas valider car sa promotion doit être non nulle.", Id);
            }
            else
            {
                string discountError = ((IDataErrorInfo)Discount).Error;
                error = string.Format("L'application de promotion {0} n'est pas valide car : {1}", Id, discountError);
            }
            return error;
        }

        private string _validateCount()
        {
            string error = null;
            if(Count <= 0)
            {
                error = string.Format("L'application de promotion {0} n'est pas valide car le nombre de promotions doit être positif", Id);
            }
            return error;
        }

        private string _validateRoom()
        {
            string error = null;
            if (Room == null)
            {
                error = string.Format("L'application de promotion {0} n'est pas valider car sa chambre doit être non nulle.", Id);
            }
            else
            {
                string roomError = ((IDataErrorInfo)Room).Error;
                error = string.Format("L'application de promotion {0} n'est pas valide car : {1}", Id, roomError);
            }
            return error;
        }
        public bool Validate()
        {
            bool discountValidates = _validateDiscount() == null;
            bool countValidates = _validateCount() == null;
            bool roomValidates = _validateRoom() == null;


            return discountValidates && countValidates && roomValidates;
        }
    }
}
