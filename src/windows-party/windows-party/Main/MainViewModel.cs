using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using System.Security;

namespace windows_party
{
    [Export(typeof(IMain))]
    public class MainViewModel : PropertyChangedBase, IMain
    {
        #region private backing fields
        private string username;
        private string password;
        #endregion

        #region public property binds
        public string Username
        { 
            get => username;
            set
            {
                username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool CanLogin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
            }
        }
        #endregion

        public void Login()
        {
            MessageBox.Show(string.Format("Hello {0}!\r\nYour password is {1}", username, password), "Log in", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
 