
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResotelApp.Models
{
    public class OptionChoice : IValidable, IDataErrorInfo
    {
        private double _discountedAmmount;
        private double _actualPrice;
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        [Required]
        public Option Option { get; set; }
        [Required]
        public DateRange TakenDates { get; set; }

        public int PeopleCount { get; set; }
        
        public double DiscountedAmmount
        {
            get
            {
                if(_discountedAmmount == -1)
                {
                    int peopleCount = 1;
                    if(PeopleCount >0)
                    {
                        peopleCount = PeopleCount;
                    }
                    _discountedAmmount = Option.BasePrice * TakenDates.Days * peopleCount - ActualPrice;   
                }
                return _discountedAmmount;
            }
        }

        public double ActualPrice
        {
            get
            {
                if(_actualPrice == -1)
                {
                    int peopleCount = 1;
                    if(PeopleCount>0)
                    {
                        peopleCount = PeopleCount;
                    }

                    if (Option.CurrentDiscount.ReduceByPercent == 0)
                    {
                        _discountedAmmount = 0;
                        _actualPrice = Option.BasePrice;
                    }
                    else if (Option.CurrentDiscount.Validity != null && Option.CurrentDiscount.ReduceByPercent > 0)
                    {
                        long minEndTicks = Math.Min(TakenDates.End.Ticks, Option.CurrentDiscount.Validity.End.Ticks);
                        long maxStartTicks = Math.Max(TakenDates.Start.Ticks, Option.CurrentDiscount.Validity.Start.Ticks);
                        int discountedDays = new DateTime(minEndTicks).Subtract(new DateTime(maxStartTicks)).Days;
                        int fullPriceDays = TakenDates.Days - discountedDays;
                        double normalPrice = Option.BasePrice * fullPriceDays * peopleCount;
                        double discountedPrice = Option.BasePrice * (1d - (Option.CurrentDiscount.ReduceByPercent / 100d)) * discountedDays * peopleCount;
                        _actualPrice = Math.Floor(normalPrice + discountedPrice);
                    } else if(Option.CurrentDiscount.Validity == null)
                    {
                        double reduceByPercent = 1;
                        if(Option.CurrentDiscount.ReduceByPercent > 0)
                        {
                            reduceByPercent = Option.CurrentDiscount.ReduceByPercent;
                        }
                        _actualPrice = Option.BasePrice * TakenDates.Days * peopleCount * (1d - (Option.CurrentDiscount.ReduceByPercent / 100d ));
                    }
                }
                return _actualPrice;
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

        public OptionChoice()
        {
            _discountedAmmount = -1;
            _actualPrice = -1;
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Option), _validateOption },
                { nameof(TakenDates), _validateTakenDates },
                { nameof(PeopleCount), _validatePeopleCount }
            };
        }

        private string _validateOption()
        {
            string error = null;
            if (Option == null)
            {
                error = string.Format("Le choix d'option {0} est invalide car l'option est requise", Id);
            }
            else
            {
                string optionError = ((IDataErrorInfo)Option).Error;
                if (optionError != null)
                {
                    error = string.Format("Le choix d'option {0} est invalide car {1}", Id, optionError);
                }
            }
            return error;
        }

        private string _validateTakenDates()
        {
            string error = null;
            string takenDatesError = ((IDataErrorInfo)TakenDates).Error;
            if(takenDatesError != null)
            {
                error = string.Format("Le choix d'option {0} est invalide car {1}", Id, takenDatesError);
            }
            return error;
        }

        private string _validatePeopleCount()
        {
            string error = null;
            if(PeopleCount < 0)
            {
                error = string.Format("Le choix d'option {0} est invalide car le nombre de personne doit être positif", Id);
            }
            else if (PeopleCount != 0 && Option != null && Option.Label != "Restauration")
            {
                error = string.Format("Le choix d'option {0} est invalide car il est associé à un nombre de personne "
                    + "mais ne concerne pas l'option restauration", Id);
            }
            return error;
        }

        public bool Validate()
        {
            bool optionValidates = _validateOption() == null;
            bool takenDatesValidates = _validateTakenDates() == null;
            bool peopleCountValidates = _validatePeopleCount() == null;

            return optionValidates && takenDatesValidates && peopleCountValidates;
        }
    }
}
