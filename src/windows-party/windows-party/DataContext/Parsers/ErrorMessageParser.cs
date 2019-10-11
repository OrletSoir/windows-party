using OrletSoir.JSON;
using System.Linq;

namespace windows_party.DataContext.Parsers
{
    public static class ErrorMessageParser
    {
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
                return null;

            // convert
            JsonSet jsonSet = jsonResult.AsSet();

            // check for [token] field and return its value
            if (jsonSet.Keys.Contains(_messageField) && jsonSet[_messageField] != null && jsonSet[_messageField].Type == JsonType.String)
                return jsonSet[_messageField].AsString();

            // something went wrong if we're here
            return null;
        }
        #endregion
    }
}
