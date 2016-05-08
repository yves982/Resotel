using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ResotelApp.ViewModels
{
    public class StartPageModel : IMessageHandler, INotifyDataErrorInfo
    {
        private IDictionary<string, ICollection<string>> _validationErrors;
        public Client Client { get; set; }
       

        public bool HasErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void Validate()
        {
            ValidationContext validationContext = new ValidationContext(Client, null, null);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(Client, validationContext, validationResults))
            {
                foreach(ValidationResult result in validationResults)
                {

                }
            }
        }

        public void Save()
        {
            Validate();
        }

        public void HandleMessage(IMessageChannel source, MessageTypes type, object data)
        {
            if (type == MessageTypes.Navigation && data.ToString() == "Next")
            {
                this.Save();
            }
            source.MessageReceived -= this.HandleMessage;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
