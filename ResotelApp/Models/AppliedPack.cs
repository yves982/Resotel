using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResotelApp.Models
{
    public class AppliedPack : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;
        private string _label;

        public int Id { get; set; }
        public Room Room { get; set; }
        public Booking Booking { get; set; }
        public Pack RoomPack { get; set; }
        public int Count { get; set; }

        [NotMapped]
        public string Label
        {
            get
            {
                if (_label == null)
                {
                    _label = $"{RoomPack.Quantity} nuits - {Room.Label}";
                }
                return _label;
            }
        }

        [NotMapped]
        public double Price
        {
            get { return RoomPack.Price; }
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

        public AppliedPack()
        {
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(RoomPack), _validateDiscount },
                { nameof(Count), _validateCount },
                { nameof(Room), _validateRoom }
            };
        }

        private string _validateDiscount()
        {
            string error = null;
            if (RoomPack == null)
            {
                error = string.Format("L'application de promotion {0} n'est pas valider car sa promotion doit être non nulle.", Id);
            }
            else
            {
                string packError = ((IDataErrorInfo)RoomPack).Error;
                if (packError != null)
                {
                    error = string.Format("L'application de promotion {0} n'est pas valide car : {1}", Id, packError);
                }
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
                if (roomError != null)
                {
                    error = string.Format("L'application de promotion {0} n'est pas valide car : {1}", Id, roomError);
                }
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
