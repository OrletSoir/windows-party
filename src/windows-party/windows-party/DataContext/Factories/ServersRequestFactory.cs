using System.Collections.Generic;

namespace windows_party.DataContext.Factories
{
    public static class ServersRequestFactory
    {
        #region private fields
        private const string _authTokenHeader = @"Authorization";
        private const string _authTokenBody = @"Bearer {0}";
        #endregion

        #region public methods
        public static Dictionary<string, string> MakeRequestHeaders(string authToken)
        {
            return new Dictionary<string, string> { { _authTokenHeader, string.Format(_authTokenBody, authToken) } };
        }
        #endregion
    }
}
