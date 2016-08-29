using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;

namespace ResotelApp.Models
{
    public class Client : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Address { get; set; }
        public List<Booking> Bookings { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

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

        public Client()
        {
            Bookings = new List<Booking>();
            _propertiesValidations = new Dictionary<string, Func<string>> {
                { nameof(FirstName), _validateFirstName },
                { nameof(LastName), _validateLastName },
                { nameof(City), _validateCity },
                { nameof(ZipCode), _validateZipCode },
                { nameof(Address), _validateAddress },
                { nameof(Email), _validateEmail },
                { nameof(Phone), _validatePhone }
            };
        }

        private string _validateFirstName()
        {
            string error = null;
            if (FirstName == null || FirstName.Length == 0)
            {
                error = string.Format("FirstName doit être renseigné sur le client: {0}", Id);
            }

            else if (FirstName.Length > 50)
            {
                error = string.Format("FirstName ne doit pas dépasser 50 caractères pour le client: {0}", Id);
            }

            return error;
        }

        private string _validateLastName()
        {
            string error = null;
            if (LastName == null || LastName.Length == 0)
            {
                error = string.Format("LastName doit être renseigné sur le client: {0}", Id);
            }

            else if (LastName.Length > 50)
            {
                error = string.Format("LastName ne doit pas dépasser 50 caractères pour le client: {0}", Id);
            }

            return error;
        }

        private string _validateCity()
        {
            string error = null;
            if (City != null && City.Length > 50)
            {
                error = string.Format("La ville ne doit pas dépasser 50 caractères pour le client: {0}", Id);
            }
            return error;
        }

        private string _validateZipCode()
        {
            string error = null;
            if (ZipCode < 0)
            {
                error = string.Format("Le code postal doit être un entier positif ou null, pour le client {0}", Id);
            }
            else if (!string.IsNullOrWhiteSpace(City) && ZipCode == 0)
            {
                error = string.Format("Le code postal 0 du client {0} est invalide pour la ville {1}", Id, City);
            }
            return error;
        }

        private string _validateAddress()
        {
            string error = null;
            if (Address != null && Address.Length > 200)
            {
                error = string.Format("L'adresse du client {0} ne doit pas dépasser 200 caractères", Id);
            }
            return error;
        }

        private string _validateEmail()
        {
            string error = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(Email))
                {
                    MailAddress validEmail = new MailAddress(Email);
                    error = null;
                }
                else
                {
                    error = null;
                }
            }
            catch (FormatException) { }

            if (error != null)
            {
                error = string.Format("L'adresse email {1} est invalide pour le client {0}", Id, Email);
            }
            return error;
        }

        private string _validatePhone()
        {
            string error = null;
            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] phoneChars = Phone != null ? Phone.ToCharArray() : new char[0];
            bool hasInvalidPhoneChar = Array.FindIndex(phoneChars, phoneChar =>
                Array.FindIndex(validChars, validChar => validChar.Equals(phoneChar)) == -1
            ) != -1;

            if (!string.IsNullOrWhiteSpace(Phone) && hasInvalidPhoneChar)
            {
                error = string.Format("Le numéro de téléphone {1} du client {0} ne peut contenir que des chiffres, sans espaces ou tirets.", Id, Phone);
            }
            else if (!string.IsNullOrWhiteSpace(Phone) && Phone.Length != 10)
            {
                error = string.Format("Le numéro de téléphone {1} du client {0} doit contenir exactement 10 chiffres.", Id, Phone);
            }
            return error;
        }

        /// <summary>Validates a Client</summary>
        /// <returns>true if the client is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool firstNameValidates = _validateFirstName() == null;
            bool lastNameValidates = _validateLastName() == null;
            bool zipCodeValidates = _validateZipCode() == null;
            bool addressValidates = _validateAddress() == null;
            bool emailValidates = _validateEmail() == null;
            bool phoneValidates = _validatePhone() == null;
            
            

            return firstNameValidates && lastNameValidates && zipCodeValidates
                 && addressValidates && emailValidates && phoneValidates;
        }
    }
}
