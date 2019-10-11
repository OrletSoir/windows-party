using System.Collections.Generic;

namespace windows_party.DataContext.Server
{
    public interface IServerResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        IEnumerable<IServerItem> Servers { get; set; }
    }
}
