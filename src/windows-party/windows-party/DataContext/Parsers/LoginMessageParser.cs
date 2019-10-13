using OrletSoir.JSON;
using System.Linq;

namespace windows_party.DataContext.Parsers
{
    public static class LoginMessageParser
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        private const string _tokenField = @"token";
        #endregion

        #region public methods
        public static string ParseResponseToken(string result)
        {
            // parse json
            IJsonVariable jsonResult = Json.Parse(result);

            // is it a set?
            if (jsonResult.Type != JsonType.Set)
            {
                Logger.Info("Invalid JSON query format: data is not a JSON set");

                return null;
            }

            // convert
            JsonSet jsonSet = jsonResult.AsSet();

            // check for [token] field and return its value
            if (jsonSet.Keys.Contains(_tokenField) && jsonSet[_tokenField] != null && jsonSet[_tokenField].Type == JsonType.String)
                return jsonSet[_tokenField].AsString();

            // something went wrong if we're here
            Logger.Info("Invalid JSON query format: field {field} was not found in the root element or is invalid", _tokenField);

            return null;
        }
        #endregion
    }
}
