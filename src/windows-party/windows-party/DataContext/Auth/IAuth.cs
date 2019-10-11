namespace windows_party.DataContext.Auth
{
    public interface IAuth
    {
        IAuthResult Authenticate(string username, string password);
    }
}
