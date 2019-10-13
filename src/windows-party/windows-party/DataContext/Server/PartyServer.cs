using System;
using System.ComponentModel;
using windows_party.DataContext.Factories;
using windows_party.DataContext.Parsers;
using windows_party.DataContext.Web;
using windows_party.Properties;

namespace windows_party.DataContext.Server
{
    public class PartyServer : IServer
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        private readonly string _url;
        private BackgroundWorker _bWorker;
        #endregion

        #region constructor/destructor
        public PartyServer(BackgroundWorker worker)
        {
            Logger.Debug("Initializing the PartyServer");

            _url = Resources.ServersUrl;
            ConfigureWorker(worker);
        }
        #endregion

        #region interface events
        public event EventHandler<ServersFetchEventArgs> FetchServerDataComplete;
        #endregion

        #region interface methods
        public IServerResult FetchServerData(string token)
        {
            Logger.Debug("Fetching server data");

            HttpResult result = Http.Get(_url, ServersRequestFactory.MakeRequestHeaders(token));

            ServerResult serverResult = new ServerResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                Logger.Debug("Server query success");

                if (!string.IsNullOrEmpty(result.Response))
                    serverResult.Servers = PartyServersListParser.ParseListItems(result.Response);
            }
            else
            {
                Logger.Debug("Server query failure");

                if (!string.IsNullOrEmpty(result.Response))
                    serverResult.Message = ErrorMessageParser.ParseErrorMessage(result.Response);
            }

            return serverResult;
        }

        public bool CanFetchServerDataAsync()
        {
            return _bWorker != null;
        }

        public void FetchServerDataAsync(string token)
        {
            Logger.Debug("Initiating async server fetch operation");

            _bWorker.RunWorkerAsync(new FetchServerDataAsyncParams { Token = token });
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
            FetchServerDataAsyncParams fetchParams = (FetchServerDataAsyncParams)e.Argument;
            e.Result = FetchServerData(fetchParams.Token);
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Debug("Background worker completed");

            ServerResult serverResult;

            // handle any unhandled errors
            if (e.Error != null)
            {
                Logger.Info("Worker has returned an error {error} with message: {message}", e.Error.GetType(), e.Error.Message);

                serverResult = new ServerResult { Success = false, Message = e.Error.Message };
            }
            else
            {
                serverResult = (ServerResult)e.Result;
            }

            FetchServerDataComplete?.Invoke(this, new ServersFetchEventArgs { ServersData = serverResult });
        }
        #endregion
    }
}
