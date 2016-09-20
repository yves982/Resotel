using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ResotelApp.Models
{
    /// <summary> A pair of Dates (both being at midnight). The thing is StartDate is before EndDate, by design.</summary>
    public class DateRange : IValidable, IDataErrorInfo, ICloneable
    {
        private Dictionary<string, Func<string>> _propertiesValidations;
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int Days
        {
            get { return End.Subtract(Start).Days; }
        }

        string IDataErrorInfo.Error
        {
            get { return _validateStartAndEnd(); }
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

        public DateRange()
        {
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(Start), _validateStartAndEnd },
                { nameof(End), _validateStartAndEnd }
            };
        }

        private string _validateStartAndEnd()
        {
            string error = null;
            if(Start == null ^ End == null)
            {
                string errPart = Start == null ? "début" : "fin";
                error = string.Format("La plage de dates {0} est invalide car elle n'a pas de date de {1}", Id, errPart);
            } else if(Start == null)
            {
                error = string.Format("La plage de dates {0} est invalide car elle n'a ni début ni fin.", Id);
            } else if(Start > End)
            {
                error = string.Format("La plage de dates {0} est invalide car son début ({1:dd/MM/yyyy HH:mm:ss}) est postérieur à sa fin ({2:dd/MM/yyyy HH:mm:ss})", Id, Start, End);
            }
            return error;
        }

        /// <summary>Validates a DateRange</summary>
        /// <returns>true if the date range is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            return _validateStartAndEnd() == null;
        }

        public object Clone()
        {
            DateRange dateRange = new DateRange {
                Start = new DateTime(Start.Ticks),
                End = new DateTime(End.Ticks)
            };
            return dateRange;
        }

        /// <summary>
        /// Indicated wether the requested date is on or within start/end bounds.
        /// </summary>
        /// <param name="date">requested date to check</param>
        /// <returns>true if this date is on or within [Start;End], false otherwise.</returns>
        public bool Contains(DateTime date)
        {
            return Start.CompareTo(date) <= 0 && End.CompareTo(date) >= 0;
        }
    }
}