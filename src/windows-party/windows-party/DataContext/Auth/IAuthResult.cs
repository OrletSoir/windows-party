namespace windows_party.DataContext.Auth
{
    public interface IAuthResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        string Token { get; set; }
    }
}
