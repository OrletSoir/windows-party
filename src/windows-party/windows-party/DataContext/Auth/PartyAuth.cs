using windows_party.DataContext.Factories;
using windows_party.DataContext.Parsers;
using windows_party.DataContext.Web;
using windows_party.Properties;

namespace windows_party.DataContext.Auth
{
    public class PartyAuth : IAuth
    {
        #region private fields
        private const string _jsonMimeType = @"application/json";
        private readonly string _url;
        #endregion

        #region constructor/destructor
        public PartyAuth()
        {
            _url = Resources.AuthUrl;
        }
        #endregion

        #region interface methods
        public IAuthResult Authenticate(string username, string password)
        {
            HttpResult result = Http.Post(_url, AuthRequestFactory.MakeJsonAuthQuery(username, password), _jsonMimeType);

            AuthResult authResult = new AuthResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Token = LoginMessageParser.ParseResponseToken(result.Response);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Message = ErrorMessageParser.ParseErrorMessage(result.Response);
            }

            // temp
            return authResult;
        }
        #endregion
    }
}
