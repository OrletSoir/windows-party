using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using windows_party.DataContext.Server;

namespace windows_party.ServerList
{
    [Export(typeof(IServerList))]
    public class ServerListViewModel: Screen, IServerList
    {
        #region private fields
        private readonly IServer _server;
        #endregion

        #region constructor/destructor
        public ServerListViewModel(IServer server)
        {
            _server = server;
        }
        #endregion

        #region interface properties
        public string Token { get; set; }
        #endregion

        #region interface events
        public event EventHandler LogoutClick;
        #endregion

        #region public property binds
        public string Message { get; private set; }
        public BindableCollection<IServerItem> Items { get; private set; }
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

            // fetch servers using the supplied token
            var serverResponse = _server.FetchServerData(Token);

            // populate items
            if (serverResponse.Success)
                Items = new BindableCollection<IServerItem>(serverResponse.Servers);
            else
                Message = serverResponse.Message;
        }
        #endregion
    }
}
