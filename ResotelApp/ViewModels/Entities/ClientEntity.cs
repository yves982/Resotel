using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.Collections.Generic;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    class ClientEntity : IEntity, INotifyPropertyChanged
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

        public string City
        {
            get { return _client.City; }
            set
            {
                _client.City = value;
                _pcs.NotifyChange();
            }
        }

        public int ZipCode
        {
            get { return _client.ZipCode; }
            set
            {
                _client.ZipCode = value;
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

        public ClientEntity(Client client)
        {
            _pcs = new PropertyChangeSupport(this);
            _client = client;
        }
    }
}
