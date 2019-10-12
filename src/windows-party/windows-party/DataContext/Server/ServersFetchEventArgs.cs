using System;

namespace windows_party.DataContext.Server
{
    public sealed class ServersFetchEventArgs: EventArgs
    {
        public IServerResult ServersData;
    }
}
