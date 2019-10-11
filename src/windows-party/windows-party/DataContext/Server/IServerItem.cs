using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.DataContext.Server
{
    public interface IServerItem
    {
        string Name { get; set; }
        string Distance { get; set; }
    }
}
