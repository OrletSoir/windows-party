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
        #region private fields
        private readonly string _url;
        private BackgroundWorker _bWorker;
        #endregion

        #region constructor/destructor
        public PartyServer(BackgroundWorker worker)
        {
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
            HttpResult result = Http.Get(_url, ServersRequestFactory.MakeRequestHeaders(token));

            ServerResult serverResult = new ServerResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.Response))
                    serverResult.Servers = PartyServersListParser.ParseListItems(result.Response);
            }
            else
            {
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
            _bWorker.RunWorkerAsync(new FetchServerDataAsyncParams { Token = token });
        }
        #endregion

        #region async worker events and methods
        private void ConfigureWorker(BackgroundWorker worker)
        {
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
            ServerResult serverResult;

            // handle any unhandled errors
            if (e.Error != null)
            {
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
