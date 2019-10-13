using OrletSoir.JSON;
using System.Linq;

namespace windows_party.DataContext.Parsers
{
    public static class ErrorMessageParser
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        private const string _messageField = @"message";
        #endregion

        #region public methods
        public static string ParseErrorMessage(string result)
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
            if (jsonSet.Keys.Contains(_messageField) && jsonSet[_messageField] != null && jsonSet[_messageField].Type == JsonType.String)
                return jsonSet[_messageField].AsString();

            // something went wrong if we're here
            Logger.Info("Invalid JSON query format: field {field} was not found in the root element or is invalid", _messageField);

            // something went wrong if we're here
            return null;
        }
        #endregion
    }
}
