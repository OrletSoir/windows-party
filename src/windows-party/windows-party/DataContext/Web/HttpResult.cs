using System.IO;
using System.Net;

namespace windows_party.DataContext.Web
{
    public sealed class HttpResult
    {
        public bool Success;
        public HttpStatusCode HttpCode;
        public string Response;

        public static HttpResult FromWebResponse(WebResponse response)
        {
            if (response == null)
                return FailedResponse();

            HttpResult result = new HttpResult { Success = false };

            if (response is HttpWebResponse)
            {
                result.HttpCode = ((HttpWebResponse)response).StatusCode;
                result.Success = result.HttpCode == HttpStatusCode.OK;
            }

            StreamReader sr = new StreamReader(response.GetResponseStream());
            result.Response = sr.ReadToEnd().Trim();

            return result;
        }

        public static HttpResult FailedResponse()
        {
            return new HttpResult { Success = false };
        }
    }
}
