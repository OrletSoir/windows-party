using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.DataContext.Auth
{
    public interface IAuth
    {
        AuthResult Authenticate(string username, string password);
    }
}
