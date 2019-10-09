using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using windows_party.Login;

namespace windows_party
{
    [Export(typeof(IMain))]
    public class MainViewModel : Conductor<IScreen>, IMain
    {
        #region public properties
        public ILogin LoginPanel { get; set; }
        #endregion

        public MainViewModel()
        {
            //ShowLogin();
        }

        protected override void OnActivate()
        {
            if (LoginPanel != null)
                LoginPanel.LoginSuccess += OnLoginSuccess;

            ShowLogin();
        }

        void OnLoginSuccess(object sender, LoginEventArgs e)
        {
            MessageBox.Show(string.Format("Login success, here's your token:\r\n{0}", e.Token), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowLogin()
        {
            ActivateItem(LoginPanel);
        }

        public void ShowServers(string token)
        {
            // nothing here yet
        }
    }
}
 