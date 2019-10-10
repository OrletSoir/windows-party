using OrletSoir.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using windows_party.Properties;

namespace windows_party.DataContext
{
    public class PartyAuth: IAuth
    {
        private const string _usernameField = @"username";
        private const string _passwordField = @"password";
        private const string _tokenField = @"token";
        private const string _messageField = @"message";
        private const string _jsonMimeType = @"application/json";

        private readonly string _url;

        public PartyAuth()
        {
            _url = Resources.AuthUrl;
        }

        public AuthResult Authenticate(string username, string password)
        {
            HttpResult result = Http.Post(_url, FabricateJsonQuery(username, password), _jsonMimeType);

            AuthResult authResult = new AuthResult { Success = result.Success };

            // error?
            if (result.Success)
            {
                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Token = ParseToken(result.Response);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Response))
                    authResult.Message = ParseMessage(result.Response);
            }

            // temp
            return authResult;
        }

        private string FabricateJsonQuery(string username, string password)
        {
            JsonSet jsonSet = new JsonSet();

            // populate fields
            jsonSet.Add(_usernameField, username);
            jsonSet.Add(_passwordField, password);

            // return
            return jsonSet.ToJsonString();
        }

        private string ParseToken(string result)
        {
            // parse json
            IJsonVariable jsonResult = Json.Parse(result);

            // is it a set?
            if (jsonResult.Type != JsonType.Set)
                return null;

            // convert
            JsonSet jsonSet = (JsonSet)jsonResult;

            // check for [token] field and return its value
            if (jsonSet.Keys.Contains(_tokenField) && jsonSet[_tokenField] != null && jsonSet[_tokenField].Type == JsonType.String)
                return jsonSet[_tokenField].AsString();

            // something went wrong if we're here
            return null;
        }

        private string ParseMessage(string result)
        {
            // parse json
            IJsonVariable jsonResult = Json.Parse(result);

            // is it a set?
            if (jsonResult.Type != JsonType.Set)
                return null;

            // convert
            JsonSet jsonSet = (JsonSet)jsonResult;

            // check for [token] field and return its value
            if (jsonSet.Keys.Contains(_messageField) && jsonSet[_messageField] != null && jsonSet[_messageField].Type == JsonType.String)
                return jsonSet[_messageField].AsString();

            // something went wrong if we're here
            return null;
        }
    }
}
