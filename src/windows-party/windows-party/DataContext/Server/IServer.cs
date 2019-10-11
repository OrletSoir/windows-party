using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.DataContext.Server
{
    public interface IServer
    {
        IServerResult FetchServerData(string token);
    }
}
