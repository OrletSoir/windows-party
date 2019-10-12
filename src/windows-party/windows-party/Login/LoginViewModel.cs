using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using windows_party.DataContext.Auth;

namespace windows_party.Login
{
    [Export(typeof(ILogin))]
    public class LoginViewModel : Screen, ILogin
    {
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
            // do the async call
            if (_auth.CanAuthenticateAsync())
            {
                Busy = true;
                _auth.AuthenticateAsync(Username, Password);
            }
        }
        #endregion

        #region activate/deactivate actions
        protected override void OnActivate()
        {
            // base call
            base.OnActivate();

            // make sure qe have the password field cleared as we start
            // for some reason this breaks the binding -- investigate later
            //Password = string.Empty;
        }
        #endregion

        #region async stuff
        private void OnAuthenticateComplete(object sender, AuthEventArgs e)
        {
            IAuthResult authResult = e.AuthResult;

            if (authResult.Success)
            {
                LoginSuccess?.Invoke(this, new LoginEventArgs { Token = authResult.Token });
            }
            else
            {
                Error = authResult.Message;
            }

            Busy = false;
        }
        #endregion
    }
}
