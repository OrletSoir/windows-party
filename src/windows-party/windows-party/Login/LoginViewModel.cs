﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security;

namespace windows_party.Login
{
    [Export(typeof(ILogin))]
    public class LoginViewModel : Screen, ILogin
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

        #region interface events
        public event EventHandler<LoginEventArgs> LoginSuccess;
        #endregion

        public void Login()
        {
            LoginEventArgs loginEventArgs = new LoginEventArgs { Token = "This is a Test!" };
            LoginSuccess?.Invoke(this, loginEventArgs);
        }
    }
}