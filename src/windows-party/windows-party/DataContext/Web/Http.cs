using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using windows_party.Properties;

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

    /**
     * This is the HTTP utility, it provides HTTP GET, HTTP POST methods for communication with the servers
     */
    public static class Http
    {
        private const string _paramGlue = @"{0}={1}";
        private const string _methodPost = @"POST";
        private const string _defaultContentType = @"application/x-www-form-urlencoded";

        public static string GetParamStr(Dictionary<string, string> parameters)
        {
            string[] posts = new string[parameters.Count];

            int i = 0;
            foreach (KeyValuePair<string, string> para in parameters)
            {
                posts[i++] = string.Format(_paramGlue, para.Key, HttpUtility.UrlEncode(para.Value));
            }

            return string.Join("&", posts);
        }

        private static WebHeaderCollection GetHttpHeaders(Dictionary<string, string> headers)
        {
            WebHeaderCollection result = new WebHeaderCollection();

            foreach (KeyValuePair<string, string> hdr in headers)
            {
                result.Add(hdr.Key, hdr.Value);
            }

            return result;
        }

        public static HttpResult Get(string uri, Dictionary<string, string> headers = null)
        {
            HttpResult result;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.UserAgent = Resources.UserAgent;

                if (headers != null && headers.Count > 0)
                    req.Headers = GetHttpHeaders(headers);

                result = HttpResult.FromWebResponse(req.GetResponse());
            }
            catch (WebException e)
            {
                result = HttpResult.FromWebResponse(e.Response);
            }
            catch (Exception e)
            {
                throw new HttpException(string.Format(Resources.HttpGetError, e.GetType(), e.Message, uri));
            }

            return result;
        }

        public static HttpResult Post(string uri, Dictionary<string, string> post)
        {
            return Post(uri, GetParamStr(post), _defaultContentType);
        }

        public static HttpResult Post(string uri, string payload, string contentType, Dictionary<string, string> headers = null)
        {
            HttpResult result;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.UserAgent = Resources.UserAgent;

                //Add these, as we're doing a POST
                req.ContentType = contentType;
                req.Method = _methodPost;

                // add other headers if specified
                if (headers != null && headers.Count > 0)
                    req.Headers = GetHttpHeaders(headers);

                //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
                byte[] bytes = Encoding.ASCII.GetBytes(payload);
                req.ContentLength = bytes.Length;
                Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();

                result = HttpResult.FromWebResponse(req.GetResponse());
            }
            catch (WebException e)
            {
                result = HttpResult.FromWebResponse(e.Response);
            }
            catch (Exception e)
            {
                throw new HttpException(string.Format(Resources.HttpPostError, e.GetType(), e.Message, uri, payload));
            }

            return result;
        }
    }
}
