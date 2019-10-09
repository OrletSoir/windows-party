using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_party.Login
{
    public class LoginEventArgs: EventArgs
    {
        public string Token;
    }

    public interface ILogin: IScreen
    {
        event EventHandler<LoginEventArgs> LoginSuccess;
    }
}
