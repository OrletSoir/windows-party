using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using windows_party.Login;
using windows_party.Properties;
using windows_party.ServerList;

namespace windows_party
{
    [Export(typeof(IMain))]
    public class MainViewModel : Conductor<IScreen>, IMain
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region interface properties
        public ILogin LoginPanel { get; set; }
        public IServerList ServerListPanel { get; set; }
        #endregion

        #region interface methods
        public void ShowLogin()
        {
            Logger.Debug("Activating LoginPanel");

            ActivateItem(LoginPanel);
        }

        public void ShowServerList(string token)
        {
            Logger.Debug("Activating ServerListPanel");

            ServerListPanel.Token = token;
            ActivateItem(ServerListPanel);
        }
        #endregion

        #region event handlers
        private void OnLoginSuccess(object sender, LoginEventArgs e)
        {
            Logger.Debug("Login event triggered");

            ShowServerList(e.Token);
        }

        private void OnLogoutClick(object sender, EventArgs e)
        {
            Logger.Debug("Logout event triggered");

            ShowLogin();
        }
        #endregion

        #region activate/deactivate actions
        protected override void OnActivate()
        {
            Logger.Debug("MainView is now active");

            DisplayName = Resources.MainViewTitle;

            if (LoginPanel != null)
                LoginPanel.LoginSuccess += OnLoginSuccess;
            else
                Logger.Error("LoginPanel is not set");

            if (ServerListPanel != null)
                ServerListPanel.LogoutClick += OnLogoutClick;
            else
                Logger.Error("ServerListPanel is not set");

            ShowLogin();
        }
        #endregion
    }
}
 