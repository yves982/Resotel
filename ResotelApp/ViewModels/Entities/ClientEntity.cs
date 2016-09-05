﻿using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class ClientEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private Client _client;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public Client Client
        {
            get { return _client; }
        }

        public int Id
        {
            get { return _client.Id; }
            set
            {
                _client.Id = value;
                _pcs.NotifyChange();
            }
        }

        public string FirstName
        {
            get { return _client.FirstName; }
            set
            {
                _client.FirstName = value;
                _pcs.NotifyChange();
            }
        }

        public string LastName
        {
            get { return _client.LastName; }
            set
            {
                _client.LastName = value;
                _pcs.NotifyChange();
            }
        }

        public DateTime BirthDate
        {
            get { return _client.BirthDate; }
            set
            {
                _client.BirthDate = value;
                _pcs.NotifyChange();
            }
        }

        public string City
        {
            get { return _client.City; }
            set
            {
                _client.City = value;
                _pcs.NotifyChange();
            }
        }

        public int? ZipCode
        {
            get
            {
                int? zipCode = null;
                if(_client.ZipCode != 0)
                {
                    zipCode = _client.ZipCode;
                }
                return zipCode;
            }
            set
            {
                if (value.HasValue)
                {
                    _client.ZipCode = value.Value;
                }
                _pcs.NotifyChange();
            }
        }

        public string Address
        {
            get { return _client.Address; }
            set
            {
                _client.Address = value;
                _pcs.NotifyChange();
            }
        }

        public List<Booking> Bookings
        {
            get { return _client.Bookings; }
            set
            {
                _client.Bookings = value;
                _pcs.NotifyChange();
            }
        }

        public string Email
        {
            get { return _client.Email; }
            set
            {
                _client.Email = value;
                _pcs.NotifyChange();
            }
        }

        public string Phone
        {
            get { return _client.Phone; }
            set
            {
                _client.Phone = value;
                _pcs.NotifyChange();
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return ((IDataErrorInfo)_client).Error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return ((IDataErrorInfo)_client)[columnName];
            }
        }

        public ClientEntity(Client client)
        {
            _pcs = new PropertyChangeSupport(this);
            _client = client;
            if(client.BirthDate.Year == 1)
            {
                // completely randomly choosen birthdate initialization (but that's better than 01/01/0001)
                _client.BirthDate = DateTime.Now.AddYears(-20);
            }
        }
    }
}
