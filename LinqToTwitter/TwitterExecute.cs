using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Web;

namespace LinqToTwitter
{
    /// <summary>
    /// Makes LINQ to Twitter more testable by isolating 
    /// execution routines that communicate with Twitter 
    /// from the rest of the logic.
    /// </summary>
    internal class TwitterExecute : ITwitterExecute
    {
        /// <summary>
        /// login name of user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// user's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

        /// <summary>
        /// Timeout (milliseconds) for writing to request 
        /// stream or reading from response stream
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Default for Timeout
        /// </summary>
        public const int DefaultTimeout = 100000;

        /// <summary>
        /// Timeout (milliseconds) to wait for a server response
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// list of response headers from query
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// backing store for UserAgent
        /// </summary>
        private string m_userAgent = "LINQ To Twitter v1.0";

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                return m_userAgent;
            }
            set
            {
                m_userAgent =
                    string.IsNullOrEmpty(value) ?
                        m_userAgent :
                        value + ";" + m_userAgent;
            }
        }

        /// <summary>
        /// True if OAuth succeeds, otherwise false.
        /// </summary>
        public bool AuthorizedViaOAuth
        {
            get
            {
                return !string.IsNullOrEmpty(OAuthTwitter.OAuthTokenSecret);
            }
        }

        /// <summary>
        /// Twitter OAuth Implementation
        /// </summary>
        public IOAuthTwitter OAuthTwitter { get; set; }

        public TwitterExecute() { }

        /// <summary>
        /// supports testing
        /// </summary>
        /// <param name="oAuthTwitter">IOAuthTwitter Mock</param>
        public TwitterExecute(IOAuthTwitter oAuthTwitter)
        {
            OAuthTwitter = oAuthTwitter;
        }

        /// <summary>
        /// generates a new TwitterQueryException from a WebException
        /// </summary>
        /// <param name="wex">Web Exception to Translate</param>
        /// <returns>new TwitterQueryException instance</returns>
        private TwitterQueryException CreateTwitterQueryException(WebException wex)
        {

            XElement responseXml;

            try
            {
                var responseStr = GetTwitterResponse(wex.Response);
                responseXml = XElement.Parse(responseStr);
            }
            catch (Exception)
            {
                string responseUri = string.Empty;

                if (wex.Response != null)
                {
                    responseUri = wex.Response.ResponseUri.ToString();
                }

                // One known reason this can happen is if you don't have an 
                // Internet connection, meaning that the response will contain
                // an HTML message, that can't be parsed as normal XML.
                responseXml = XElement.Parse(
@"<hash>
  <request>" + responseUri + @"</request>
  <error>See Inner Exception Details for more information.</error>
</hash>");
            }

            return new TwitterQueryException("Error while querying Twitter.", wex)
            {
                HttpError = 
                    wex != null && wex.Response != null ? 
                        wex.Response.Headers["Status"] : 
                        string.Empty,
                Response = new TwitterHashResponse
                {
                    Request = responseXml.Element("request").Value,
                    Error = responseXml.Element("error").Value
                }
            };
        }

        /// <summary>
        /// gets WebResponse contents from Twitter
        /// </summary>
        /// <param name="resp">WebResponse to extract string from</param>
        /// <returns>XML string response from Twitter</returns>
        private string GetTwitterResponse(WebResponse resp)
        {
            StringReader txtRdr;

            using (var strm = resp.GetResponseStream())
            {
                var strmRdr = new StreamReader(strm);
                txtRdr = new StringReader(strmRdr.ReadToEnd());
            }

            //
            // This code assumes that the caller will find that
            // name/value pairs are easier to work with via 
            // LINQ to Objects over an IEnumerable collection.
            //

            var responseHeaders = new Dictionary<string, string>();
            
            foreach (string key in resp.Headers.Keys)
            {
                responseHeaders.Add(key, resp.Headers[key].ToString());
            }

            ResponseHeaders = responseHeaders;

            return txtRdr.ReadToEnd();
        }

        /// <summary>
        /// either calls the appropriate strategy method for processing results
        /// or intercepts an error and throws an exception
        /// </summary>
        /// <param name="requestProcessor">strategy class for processing XML response</param>
        /// <param name="responseStr">XML string response from Twitter</param>
        /// <returns></returns>
        private static IList ProcessResults(IRequestProcessor requestProcessor, string responseStr, string status)
        {
            var responseXml = XElement.Parse(responseStr);

            if (responseXml.Name == "hash")
            {
                throw new TwitterQueryException("Error while querying Twitter.")
                {
                    HttpError = status,
                    Response = new TwitterHashResponse
                    {
                        Request = responseXml.Element("request").Value,
                        Error = responseXml.Element("error").Value
                    }
                };
            }

            return requestProcessor.ProcessResults(responseXml);
        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        public IList QueryTwitter(string url, IRequestProcessor requestProcessor)
        {
            if (AuthorizedViaOAuth)
            {
                string outUrl;
                string queryString;
                OAuthTwitter.GetOAuthQueryString(HttpMethod.GET, url, out outUrl, out queryString);
                url = outUrl + "?" + queryString;
            }

            var req = HttpWebRequest.Create(url) as HttpWebRequest;

            if (!AuthorizedViaOAuth)
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            req.UserAgent = UserAgent;

            if (ReadWriteTimeout > 0)
            {
                req.ReadWriteTimeout = ReadWriteTimeout;
            }

            if (Timeout > 0)
            {
                req.Timeout = Timeout;
            }

            string responseXml = string.Empty;
            string httpStatus = string.Empty;
            WebResponse resp = null;

            try
            {
                resp = req.GetResponse();
                responseXml = GetTwitterResponse(resp);
                httpStatus = resp.Headers["Status"];
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }

            if (new Uri(url).LocalPath.EndsWith("json"))
            {
                var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(responseXml));
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);

                var doc = new XmlDocument();
                doc.Load(reader);
                responseXml = doc.OuterXml;
            }

            return ProcessResults(requestProcessor, responseXml, httpStatus);
        }

        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="fileName">name of file to upload</param>
        /// <param name="url">url to upload to</param>
        /// <returns>IQueryable</returns>
        public IList PostTwitterFile(string filePath, Dictionary<string, string> parameters, string url, IRequestProcessor requestProcessor)
        {
            var file = Path.GetFileName(filePath);

            string imageType;

            switch (Path.GetExtension(file).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    imageType = "jpg";
                    break;
                case ".gif":
                    imageType = "gif";
                    break;
                case ".png":
                    imageType = "png";
                    break;
                default:
                    throw new ArgumentException(
                        "Can't recognize the extension of the file you're uploading. Please choose either a *.gif, *.jpg, *.jpeg, or *.png file.", filePath);
            }

            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
            var contentDisposition = string.Format("Content-Disposition:form-data); name=\"image\"); filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", file, imageType);
            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

            var formDataSB = new StringBuilder();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var param in parameters)
                {
                    formDataSB.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", contentBoundaryBase, param.Key, param.Value);
                }
            }

            byte[] fileBytes = null;
            string fileByteString = null;
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4096];
                var memStr = new MemoryStream();
                memStr.Position = 0;
                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStr.Write(buffer, 0, bytesRead);
                }

                memStr.Position = 0;
                fileByteString = encoding.GetString(memStr.GetBuffer());
            }

            fileBytes =
                encoding.GetBytes(
                    formDataSB.ToString() +
                    beginContentBoundary +
                    contentDisposition +
                    fileByteString +
                    endContentBoundary);

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
            req.PreAuthenticate = true;
            req.AllowWriteStreamBuffering = true;
            req.Method = "POST";
            req.UserAgent = UserAgent;
            req.ContentLength = fileBytes.Length;

            if (ReadWriteTimeout > 0)
            {
                req.ReadWriteTimeout = ReadWriteTimeout;
            }

            if (Timeout > 0)
            {
                req.Timeout = Timeout;
            }

            if (AuthorizedViaOAuth)
            {
                req.Headers.Add(
                    HttpRequestHeader.Authorization,
                    OAuthTwitter.GetOAuthAuthorizationHeader(url, null));
            }
            else
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            string responseXml = null;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(fileBytes, 0, fileBytes.Length);
                reqStream.Flush();
            }

            string httpStatus = string.Empty;
            WebResponse resp = null;

            try
            {
                resp = req.GetResponse();

                httpStatus = resp.Headers["Status"];

                using (var respStream = resp.GetResponseStream())
                using (var respRdr = new StreamReader(respStream))
                {
                    responseXml = respRdr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }

            return ProcessResults(requestProcessor, responseXml, httpStatus);
        }

        /// <summary>
        /// Url Encodes for both OAuth and Basic Authentication
        /// </summary>
        /// <param name="value">string to be encoded</param>
        /// <returns>UrlEncoded string</returns>
        public string TwitterParameterUrlEncode(string value)
        {
            string ReservedChars = @"`!@#$%^&*()_-+=.~,:;'?/|\[] ";
            string UnReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            
            var result = new StringBuilder();

            if (string.IsNullOrEmpty(value))
                return string.Empty;

            foreach (var symbol in value)
            {
                if (UnReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else if (ReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
                else
                {
                    var encoded = HttpUtility.UrlEncode(symbol.ToString());

                    if (!string.IsNullOrEmpty(encoded))
                    {
                        result.Append(encoded);
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        public IList ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            string paramsJoined = string.Empty;

            paramsJoined =
                string.Join(
                    "&",
                    (from param in parameters
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + TwitterParameterUrlEncode(param.Value))
                     .ToArray());

            url += "?" + paramsJoined;

            var req = WebRequest.Create(url) as HttpWebRequest;

            if (AuthorizedViaOAuth)
            {
                req.Headers.Add(
                    HttpRequestHeader.Authorization,
                    OAuthTwitter.GetOAuthAuthorizationHeader(url, null));
            }
            else
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            var bytes = Encoding.UTF8.GetBytes(paramsJoined);
            req.ContentLength = bytes.Length;
            req.Method = "POST";
            req.ContentType = "x-www-form-urlencoded";
            req.UserAgent = UserAgent;
            req.ServicePoint.Expect100Continue = false;

            if (ReadWriteTimeout > 0)
            {
                req.ReadWriteTimeout = ReadWriteTimeout;
            }

            if (Timeout > 0)
            {
                req.Timeout = Timeout;
            }

            string httpStatus = string.Empty;
            string responseXml = string.Empty;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);

                WebResponse resp = null;

                try
                {
                    resp = req.GetResponse();

                    httpStatus = resp.Headers["Status"];

                    using (var respStream = resp.GetResponseStream())
                    using (var respRdr = new StreamReader(respStream))
                    {
                        responseXml = respRdr.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    var twitterQueryEx = CreateTwitterQueryException(wex);
                    throw twitterQueryEx;
                }
                finally
                {
                    if (resp != null)
                    {
                        resp.Close();
                    }
                }
            }

            return ProcessResults(requestProcessor, responseXml, httpStatus);
        }
    }
}
