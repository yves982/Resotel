using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text;

namespace ResotelApp.Models
{
    class User : IValidable, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _propertiesValidations;

        public int Id { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Service { get; set; }

        public bool Manager { get; set; }

        [Required]
        public UserRights Rights { get; set; }

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

        public User()
        {
            _propertiesValidations = new Dictionary<string, Func<string>>
            {
                { nameof(Password), _validatePassword },
                { nameof(Login), _validateLogin },
                { nameof(FirstName), _validateFirstName },
                { nameof(LastName), _validateLastName },
                { nameof(Email), _validateEmail },
                { nameof(Service), _validateService }
            };
        }

        private string _validatePassword()
        {
            string error = null;
            if(string.IsNullOrWhiteSpace(Password))
            {
                error = string.Format("L'utilisateur {0} est invalide, car un mot de passe est requis.", Id);
            }
            else if(Password != null && Password.Length > 256)
            {
                error = string.Format("L'utilisateur {0} est invalide, car son mot de passe (hash) ne doit pas dépasser 256 caractères.", Id);
            }
            return error;
        }

        private string _validateLogin()
        {
            string error = null;
            if (string.IsNullOrWhiteSpace(Login))
            {
                error = string.Format("L'utilisateur {0} est invalide, car un login est requis.", Id);
            }
            else if (Login != null && Login.Length > 50)
            {
                error = string.Format("L'utilisateur {0} est invalide, car son login ne doit pas dépasser 50 caractères.", Id);
            }
            return error;
        }

        private string _validateFirstName()
        {
            string error = null;
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                error = string.Format("L'utilisateur {0} est invalide, car un prénom est requis.", Id);
            }
            else if (FirstName != null && FirstName.Length > 50)
            {
                error = string.Format("L'utilisateur {0} est invalide, car son prénom ne doit pas dépasser 50 caractères.", Id);
            }
            return error;
        }

        private string _validateLastName()
        {
            string error = null;
            if (string.IsNullOrWhiteSpace(LastName))
            {
                error = string.Format("L'utilisateur {0} est invalide, car un nom est requis.", Id);
            }
            else if (LastName != null && LastName.Length > 50)
            {
                error = string.Format("L'utilisateur {0} est invalide, car son nom ne doit pas dépasser 50 caractères.", Id);
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
                error = string.Format("L'adresse email {1} est invalide pour l'utilisateur {0}", Id, Email);
            }
            return error;
        }

        private string _validateService()
        {
            string error = null;
            if (string.IsNullOrWhiteSpace(Service))
            {
                error = string.Format("L'utilisateur {0} est invalide, car son service est requis.", Id);
            }
            else if (Service != null && Service.Length > 50)
            {
                error = string.Format("L'utilisateur {0} est invalide, car son service ne doit pas dépasser 50 caractères.", Id);
            }
            return error;
        }

        /// <summary>Validates a User</summary>
        /// <returns>true if the user is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing an instance</remarks>
        public bool Validate()
        {
            bool passwordValidates = _validatePassword() == null;
            bool loginValidates = _validateLogin() == null;
            bool firstNameValidates = _validateFirstName() == null;
            bool lastNameValidates = _validateLastName() == null;
            bool emailValidates = _validateEmail() == null;
            bool serviceValidates = _validateService() == null;

            return passwordValidates && loginValidates && firstNameValidates
                && lastNameValidates && emailValidates && serviceValidates;
        }
    }
}