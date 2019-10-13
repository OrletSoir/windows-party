using System;
using System.ComponentModel;
using windows_party.DataContext.Factories;
using windows_party.DataContext.Parsers;
using windows_party.DataContext.Web;
using windows_party.Properties;

namespace windows_party.DataContext.Auth
{
    public class PartyAuth : IAuth
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        private const string _noWorker = @"No background worker provided.";
        private const string _jsonMimeType = @"application/json";
        private readonly string _url;
        private BackgroundWorker _bWorker;
        #endregion

        #region constructor/destructor
        public PartyAuth(BackgroundWorker worker)
        {
            Logger.Debug("Initializing the PartyAuth");

            _url = Resources.AuthUrl;
            ConfigureWorker(worker);
        }
        #endregion

        #region interface events
        public event EventHandler<AuthEventArgs> AuthenticateComplete;
        #endregion

        #region interface methods
        public IAuthResult Authenticate(string username, string password)
        {
            Logger.Debug("Authentication in progress");

            HttpResult result = Http.Post(_url, AuthRequestFactory.MakeJsonAuthQuery(username, password), _jsonMimeType);

            AuthResult authResult = new AuthResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                Logger.Debug("Authentication HTTP query success");

                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Token = LoginMessageParser.ParseResponseToken(result.Response);
            }
            else
            {
                Logger.Debug("Authentication HTTP query failure");

                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Message = ErrorMessageParser.ParseErrorMessage(result.Response);
            }

            // temp
            return authResult;
        }

        public bool CanAuthenticateAsync()
        {
            return _bWorker != null;
        }

        public void AuthenticateAsync(string username, string password)
        {
            Logger.Debug("Initiating async authentication");

            _bWorker.RunWorkerAsync(new AuthAsyncParams { Username = username, Password = password });
        }
        #endregion

        #region async worker events and methods
        private void ConfigureWorker(BackgroundWorker worker)
        {
            if (worker == null)
            {
                Logger.Error("Worker for PartyAuth not specified");
                return;
            }

            Logger.Debug("Configuring the background worker");

            _bWorker = worker;

            _bWorker.DoWork += OnDoWork;
            _bWorker.RunWorkerCompleted += OnRunWorkerCompleted;
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            AuthAsyncParams authParams = (AuthAsyncParams)e.Argument;
            e.Result = Authenticate(authParams.Username, authParams.Password);
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Debug("Background worker completed");

            AuthResult authResult;

            // handle any unhandled errors
            if (e.Error != null)
            {
                Logger.Info("Worker has returned an error {error} with message: {message}", e.Error.GetType(), e.Error.Message);

                authResult = new AuthResult { Success = false, Message = e.Error.Message};
            }
            else
            {
                authResult = (AuthResult)e.Result;
            }

            AuthenticateComplete?.Invoke(this, new AuthEventArgs { AuthResult = authResult });
        }
        #endregion
    }
}