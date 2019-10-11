using OrletSoir.JSON;

namespace windows_party.DataContext.Factories
{
    public static class AuthRequestFactory
    {
        #region private fields
        private const string _usernameField = @"username";
        private const string _passwordField = @"password";
        #endregion

        #region public methods
        public static string MakeJsonAuthQuery(string username, string password)
        {
            JsonSet jsonSet = new JsonSet();

            // populate fields
            jsonSet.Add(_usernameField, username);
            jsonSet.Add(_passwordField, password);

            // return
            return jsonSet.ToJsonString();
        }
        #endregion
    }
}
