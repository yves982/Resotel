using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// ViewModel pendant of User with changes notifications
    /// </summary>
    class UserEntity : IEntity, INotifyPropertyChanged, IDataErrorInfo
    {
        private User _user;
        private PropertyChangeSupport _pcs;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public int Id
        {
            get { return _user.Id; }

            set
            {
                _user.Id = value;
                _pcs.NotifyChange();
            }
        }

        public string Password
        {
            get { return _user.Password; }

            set
            {
                _user.Password = value;
                _pcs.NotifyChange();
            }
        }

        public string Login
        {
            get { return _user.Login; }

            set
            {
                _user.Login = value;
                _pcs.NotifyChange();
            }
        }

        public string FirstName
        {
            get { return _user.FirstName; }

            set
            {
                _user.FirstName = value;
                _pcs.NotifyChange();
            }
        }

        public string LastName
        {
            get { return _user.LastName; }

            set
            {
                _user.LastName = value;
                _pcs.NotifyChange();
            }
        }

        public string Email
        {
            get { return _user.Email; }

            set
            {
                _user.Email = value;
                _pcs.NotifyChange();
            }
        }

        public string Service
        {
            get { return _user.Service; }

            set
            {
                _user.Service = value;
                _pcs.NotifyChange();
            }
        }

        public bool Manager
        {
            get { return _user.Manager; }

            set
            {
                _user.Manager = value;
                _pcs.NotifyChange();
            }
        }

        public UserRights Rights
        {
            get { return _user.Rights; }

            set
            {
                _user.Rights = value;
                _pcs.NotifyChange();
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return ((IDataErrorInfo)_user).Error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return ((IDataErrorInfo)_user)[columnName];
            }
        }

        public UserEntity(User user)
        {
            _pcs = new PropertyChangeSupport(this);
            _user = user;
        }
    }
}
