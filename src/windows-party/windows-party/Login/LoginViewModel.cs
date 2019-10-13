using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using windows_party.DataContext.Auth;

namespace windows_party.Login
{
    [Export(typeof(ILogin))]
    public class LoginViewModel : Screen, ILogin
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        private readonly IAuth _auth;
        #endregion

        #region private backing fields
        private string username;
        private string password;
        private string error;
        private bool busy;
        #endregion

        #region constructor/destructor
        public LoginViewModel(IAuth auth)
        {
            Logger.Debug("Initializing the LoginViewModel");

            busy = false;
            _auth = auth;

            // attach our async call complete event handler
            _auth.AuthenticateComplete += OnAuthenticateComplete;
        }
        #endregion

        #region interface events
        public event EventHandler<LoginEventArgs> LoginSuccess;
        #endregion

        #region public property binds
        public string Username
        {
            get => username;
            set
            {
                // reset error
                error = string.Empty;
                username = value;

                NotifyOfPropertyChange(() => Error);
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Password
        {
            get => password;
            set
            {
                // reset error
                error = string.Empty;
                password = value;

                NotifyOfPropertyChange(() => Error);
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Error
        {
            get => error;
            protected set
            {
                error = value;

                NotifyOfPropertyChange(() => Error);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool Busy
        {
            get => busy;
            protected set
            {
                busy = value;

                NotifyOfPropertyChange(() => Busy);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool CanLogin
        {
            get
            {
                return !busy && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
            }
        }
        #endregion

        #region method binds
        public void Login()
        {
            Logger.Debug("Login button clicked");

            // do the async call
            if (_auth.CanAuthenticateAsync())
            {
                Logger.Debug("Attempting authentification");

                Busy = true;
                _auth.AuthenticateAsync(Username, Password);
            }
        }
        public void ExecuteFilterView(Key key)
        {
            if (key == Key.Enter && CanLogin)
            {
                Login();
            }
        }

        #endregion

        #region activate/deactivate actions
        protected override void OnActivate()
        {
            Logger.Debug("LoginViewModel is now active");

            // base call
            base.OnActivate();

            // make sure qe have the password field cleared as we start/restart
            Password = null;
        }
        #endregion

        #region async stuff
        private void OnAuthenticateComplete(object sender, AuthEventArgs e)
        {
            Logger.Debug("Authentication complete event triggered");

            IAuthResult authResult = e.AuthResult;

            if (authResult.Success)
            {
                Logger.Info("Authentication successful"); // don't log the token, d'uh

                LoginSuccess?.Invoke(this, new LoginEventArgs { Token = authResult.Token });
            }
            else
            {
                Logger.Info("Authentication failed: {message}", authResult.Message);

                Error = authResult.Message;
            }

            Busy = false;
        }
        #endregion
    }
}
