using System;

namespace windows_party.DataContext.Auth
{
    public interface IAuth
    {
        IAuthResult Authenticate(string username, string password);

        bool CanAuthenticateAsync();
        void AuthenticateAsync(string username, string password);
        event EventHandler<AuthEventArgs> AuthenticateComplete;
    }
}
