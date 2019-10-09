using Caliburn.Micro;

namespace windows_party
{
    public interface IMain
    {
        Login.ILogin LoginPanel { get; set; }

        void ShowLogin();
        void ShowServers(string token);
    }
}