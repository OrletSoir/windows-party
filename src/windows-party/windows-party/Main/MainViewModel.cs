using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using windows_party.Login;
using windows_party.ServerList;

namespace windows_party
{
    [Export(typeof(IMain))]
    public class MainViewModel : Conductor<IScreen>, IMain
    {
        #region interface properties
        public ILogin LoginPanel { get; set; }
        public IServerList ServerListPanel { get; set; }
        #endregion

        #region interface methods
        public void ShowLogin()
        {
            ActivateItem(LoginPanel);
        }

        public void ShowServerList(string token)
        {
            ServerListPanel.Token = token;
            ActivateItem(ServerListPanel);
        }
        #endregion

        #region event handlers
        private void OnLogoutClick(object sender, EventArgs e)
        {
            ShowLogin();
        }

        private void OnLoginSuccess(object sender, LoginEventArgs e)
        {
            ShowServerList(e.Token);
        }
        #endregion

        #region activate/deactivate actions
        protected override void OnActivate()
        {
            if (LoginPanel != null)
                LoginPanel.LoginSuccess += OnLoginSuccess;

            if (ServerListPanel != null)
                ServerListPanel.LogoutClick += OnLogoutClick;

            ShowLogin();
        }
        #endregion
    }
}
 