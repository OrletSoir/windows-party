using System.IO;
using System.Net;

namespace windows_party.DataContext.Web
{
    public sealed class HttpResult
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region actual important fields
        public bool Success;
        public HttpStatusCode HttpCode;
        public string Response;
        #endregion

        #region factory methods
        public static HttpResult FromWebResponse(WebResponse response)
        {
            if (response == null)
                return FailedResponse();

            HttpResult result = new HttpResult { Success = false };

            if (response is HttpWebResponse)
            {
                Logger.Debug("HTTP query returned {code} {description}", ((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription);

                result.HttpCode = ((HttpWebResponse)response).StatusCode;
                result.Success = result.HttpCode == HttpStatusCode.OK;
            }
            else
                Logger.Debug("Unknown WebResponse: {type}", response.GetType());

            StreamReader sr = new StreamReader(response.GetResponseStream());
            result.Response = sr.ReadToEnd().Trim();

            return result;
        }

        public static HttpResult FailedResponse()
        {
            return new HttpResult { Success = false };
        }
        #endregion
    }
}
