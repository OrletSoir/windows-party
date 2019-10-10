using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.Data
{
    public class ServerItem
    {
        #region public properties
        public string Name { get; private set; }
        public string Distance { get; private set; }
        #endregion

        #region constructior/destructor
        public ServerItem(string name, int distance)
        {
            Name = name;
            Distance = distance.ToString();
        }
        #endregion
    }
}
