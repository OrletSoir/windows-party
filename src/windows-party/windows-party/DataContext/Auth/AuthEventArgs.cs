using System;

namespace windows_party.DataContext.Auth
{
    public sealed class AuthEventArgs : EventArgs
    {
        public IAuthResult AuthResult;
    }
}
