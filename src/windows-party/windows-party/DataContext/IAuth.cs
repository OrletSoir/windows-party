using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.DataContext
{
    public sealed class AuthResult
    {
        public bool Success;
        public string Message;
        public string Token;
    }

    public interface IAuth
    {
        AuthResult Authenticate(string username, string password);
    }
}
