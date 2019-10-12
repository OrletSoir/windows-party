using System;

namespace windows_party.DataContext.Server
{
    public interface IServer
    {
        IServerResult FetchServerData(string token);

        bool CanFetchServerDataAsync();
        void FetchServerDataAsync(string token);
        event EventHandler<ServersFetchEventArgs> FetchServerDataComplete;
    }
}
