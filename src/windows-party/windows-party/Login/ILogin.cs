using Caliburn.Micro;
using System;

namespace windows_party.Login
{
    public interface ILogin: IScreen
    {
        event EventHandler<LoginEventArgs> LoginSuccess;
    }
}
