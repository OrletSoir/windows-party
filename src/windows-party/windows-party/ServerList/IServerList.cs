using Caliburn.Micro;
using System;

namespace windows_party.ServerList
{
    public interface IServerList: IScreen
    {
        string Token { get; set; }

        event EventHandler LogoutClick;
    }
}
