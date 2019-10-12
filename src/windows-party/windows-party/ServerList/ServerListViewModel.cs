using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using windows_party.DataContext.Server;

namespace windows_party.ServerList
{
    [Export(typeof(IServerList))]
    public class ServerListViewModel : Screen, IServerList
    {
        #region private fields
        private readonly IServer _server;
        #endregion

        #region private backing fields
        private string message;
        private BindableCollection<IServerItem> items;
        #endregion

        #region constructor/destructor
        public ServerListViewModel(IServer server)
        {
            _server = server;

            // attach our async call complete event handler
            _server.FetchServerDataComplete += OnFetchServerDataComplete;
        }
        #endregion

        #region interface properties
        public string Token { get; set; }
        #endregion

        #region interface events
        public event EventHandler LogoutClick;
        #endregion

        #region public property binds
        public string Message
        {
            get => message;
            protected set
            {
                message = value;

                NotifyOfPropertyChange(() => Message);
            }
        }

        public BindableCollection<IServerItem> Items
        {
            get => items;
            protected set
            {
                items = value;

                NotifyOfPropertyChange(() => Items);
            }
        }
        #endregion

        #region method binds
        public void Logout()
        {
            LogoutClick?.Invoke(this, new EventArgs());
        }
        #endregion

        #region activate/deactivate actions
        protected override void OnActivate()
        {
            // base call
            base.OnActivate();

            // do the async call
            if (_server.CanFetchServerDataAsync())
                _server.FetchServerDataAsync(Token);
        }
        #endregion

        #region async stuff
        private void OnFetchServerDataComplete(object sender, ServersFetchEventArgs e)
        {
            var serverResponse = e.ServersData;

            // populate items
            if (serverResponse.Success)
                Items = new BindableCollection<IServerItem>(serverResponse.Servers);
            else
                Message = serverResponse.Message;
        }
        #endregion
    }
}
