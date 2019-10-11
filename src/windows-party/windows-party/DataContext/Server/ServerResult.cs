using System.Collections.Generic;

namespace windows_party.DataContext.Server
{
    public sealed class ServerResult : IServerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<IServerItem> Servers { get; set; }
    }
}
