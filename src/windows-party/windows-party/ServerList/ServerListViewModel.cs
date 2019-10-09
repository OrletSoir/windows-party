using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using windows_party.Data;

namespace windows_party.ServerList
{
    [Export(typeof(IServerList))]
    public class ServerListViewModel: Screen, IServerList
    {
        #region interface properties
        public string Token { get; set; }
        #endregion

        #region interface events
        public event EventHandler LogoutClick;
        #endregion

        #region public property binds
        public BindableCollection<ServerItem> Items { get; private set; }
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

            // populate items
            Random rnd = new Random();

            Items = new BindableCollection<ServerItem>{
                new ServerItem(Guid.NewGuid().ToString(), rnd.Next(0, 2500)),
                new ServerItem(Guid.NewGuid().ToString(), rnd.Next(0, 2500)),
                new ServerItem(Guid.NewGuid().ToString(), rnd.Next(0, 2500)),
                new ServerItem(Guid.NewGuid().ToString(), rnd.Next(0, 2500))
            };
        }
        #endregion
    }
}
