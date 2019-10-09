namespace windows_party
{
    public interface IMain
    {
        Login.ILogin LoginPanel { get; set; }
        ServerList.IServerList ServerListPanel { get; set; }

        void ShowLogin();
        void ShowServerList(string token);
    }
}