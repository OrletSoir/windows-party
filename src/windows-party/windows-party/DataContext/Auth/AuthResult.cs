namespace windows_party.DataContext.Auth
{
    public sealed class AuthResult: IAuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
