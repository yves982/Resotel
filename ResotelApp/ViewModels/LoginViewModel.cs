using ResotelApp.Models;
using ResotelApp.Repositories;
using ResotelApp.Utils;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResotelApp.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private PropertyChangeSupport _pcs;
        private string _login;
        private bool _resultReady;
        public string _loginResult;
        private IUITimer _timer;
        private SecureString _securePassword;
        private string _title;
        private User _user;


        private DelegateCommandAsync<object> _loginCommand;
        private DelegateCommand<IUITimer> _loadCommand;

        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                _unlockLoginIfNeeded();
                _pcs.NotifyChange();
            }
        }

        public bool ResultReady
        {
            get { return _resultReady; }
            set
            {
                _resultReady = value;
                _pcs.NotifyChange();
            }
        }

        public string LoginResult
        {
            get { return _loginResult; }
            set
            {
                _loginResult = value;
                _pcs.NotifyChange();
            }
        }

        public SecureString SecurePassword
        {
            get { return _securePassword; }
            set
            {
                _securePassword = value;
                _unlockLoginIfNeeded();
                _pcs.NotifyChange();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _pcs.NotifyChange();
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new DelegateCommandAsync<object>(_onLogin, false);
                }
                return _loginCommand;
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                if(_loadCommand == null)
                {
                    _loadCommand = new DelegateCommand<IUITimer>(_onLoad);
                }
                return _loadCommand;
            }
        }

        public string Error
        {
            get
            {
                string error = null;
                string pwdError = this[nameof(SecurePassword)];
                string loginError = this[nameof(Login)];
                if(pwdError != null && loginError != null)
                {
                    error = string.Format("{0};{1}", pwdError, loginError); 
                } else if(pwdError != null)
                {
                    error = pwdError;
                } else if(loginError != null)
                {
                    error = loginError;
                }


                return error;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(SecurePassword):
                        if(_securePassword == null || _securePassword.Length == 0)
                        {
                            error = @"Le champ ""Mot de passe"" est requis.";
                        }
                        break;
                    case nameof(Login):
                        if(_login == null || _login.Length == 0 )
                        {
                            error = @"Le champ ""Login"" est requis.";
                        }
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public LoginViewModel()
        {
            _pcs = new Utils.PropertyChangeSupport(this);
            _loginResult = "";
            _resultReady = false;
            _title = "Resotel - Login";
        }

        private void _unlockLoginIfNeeded()
        {
            if (_login != null && _login.Length > 0
                    && _securePassword != null && SecureStringUtil.Read(_securePassword).Length > 0
                    && !_loginCommand.CanExecute(null))
            {
                _loginCommand.ChangeCanExecute();
            }
            else if( (
                        _login == null || _login.Length == 0
                        || _securePassword == null || SecureStringUtil.Read(_securePassword).Length == 0
                     ) && _loginCommand.CanExecute(null)
            )
            {
                _loginCommand.ChangeCanExecute();
            }
        }

        private void _onLoad(IUITimer timer)
        {
            _timer = timer;
        }

        private async Task _onLogin(object ignore)
        {
            User user = await UserRepository.FindByLoginAsync(_login);

            if (user == null)
            {
                LoginResult = "User Not found";
                ResultReady = true;
                return;
            }
            bool passwordMatch = user.Password.Equals(HashManager.SHA256(SecureStringUtil.Read(_securePassword)), StringComparison.InvariantCultureIgnoreCase);

            bool hasRights = (user.Rights & UserRights.Booking) == UserRights.Booking;
            if (passwordMatch && hasRights)
            {
                _user = user;
                LoginResult = "Success, loading...";
                ResultReady = true;
                _timer.IntervalMS = 1400;
                _timer.Elapsed += _showMainView;
                _timer.IsEnabled = true;
            }
            else if (!passwordMatch)
            {
                LoginResult = "Invalid user name or password";
                ResultReady = true;
            } else
            {
                LoginResult = "Unauthorized";
                ResultReady = true;
            }
        }

        private void _showMainView(object sender, EventArgs e)
        {
            UserEntity userEntity = new UserEntity(_user);
            MainWindowViewModel mainViewModel = new MainWindowViewModel(userEntity);
            ViewDriverProvider.ViewDriver.CloseAndShowNewMainWindow<MainWindowViewModel>(mainViewModel);
            _timer.IsEnabled = false;
        }
    }
}
