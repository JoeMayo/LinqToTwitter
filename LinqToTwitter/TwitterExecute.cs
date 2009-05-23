using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;

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
            var responseStr = GetTwitterResponse(wex.Response);

            XElement responseXml;

            try
            {
                responseXml = XElement.Parse(responseStr);
            }
            catch (Exception)
            {
                // One known reason this can happen is if you don't have an 
                // Internet connection, meaning that the response will contain
                // an HTML message, that can't be parsed as normal XML.
                responseXml = XElement.Parse(
@"<hash>
  <request>" + wex.Response.ResponseUri + @"</request>
  <error>See Inner Exception Details for more information.</error>
</hash>");
            }

            return new TwitterQueryException("Error while querying Twitter.", wex)
            {
                HttpError = wex.Response.Headers["Status"],
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

            return txtRdr.ReadToEnd();
        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        public IQueryable QueryTwitter(string url, IRequestProcessor requestProcessor)
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

            WebResponse resp = null;
            string responseStr = null;

            try
            {
                resp = req.GetResponse();
                responseStr = GetTwitterResponse(resp);
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

            XElement statusXml = null;

            if (new Uri(url).LocalPath.EndsWith("json"))
            {
                var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(responseStr));
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);

                var doc = new XmlDocument();
                doc.Load(reader);

                statusXml = XElement.Parse(doc.OuterXml);
            }
            else
            {
                statusXml = XElement.Parse(responseStr);
            }

            return requestProcessor.ProcessResults(statusXml);
        }

        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="fileName">name of file to upload</param>
        /// <param name="url">url to upload to</param>
        /// <returns>IQueryable</returns>
        public IQueryable PostTwitterFile(string filePath, string url, IRequestProcessor requestProcessor)
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

            byte[] fileBytes = null;
            string fileByteString = null;

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
                fileByteString = Encoding.GetEncoding("iso-8859-1").GetString(memStr.GetBuffer());
            }

            fileBytes =
                Encoding.GetEncoding("iso-8859-1").GetBytes(
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

            string responseXML = null;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(fileBytes, 0, fileBytes.Length);
                reqStream.Flush();
            }

            WebResponse resp = null;

            try
            {
                resp = req.GetResponse();

                using (var respStream = resp.GetResponseStream())
                using (var respRdr = new StreamReader(respStream))
                {
                    responseXML = respRdr.ReadToEnd();
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

            var responseXElem = XElement.Parse(responseXML);
            var results = requestProcessor.ProcessResults(responseXElem);
            return results;
        }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        public IQueryable ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            string paramsJoined = string.Empty;

            paramsJoined =
                string.Join(
                    "&",
                    (from param in parameters
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + OAuthTwitter.OAuthParameterUrlEncode(param.Value))
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

            string responseXML;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);

                WebResponse resp = null;

                try
                {
                    resp = req.GetResponse();

                    using (var respStream = resp.GetResponseStream())
                    using (var respRdr = new StreamReader(respStream))
                    {
                        responseXML = respRdr.ReadToEnd();
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

            var responseXElem = XElement.Parse(responseXML);
            var results = requestProcessor.ProcessResults(responseXElem);
            return results;
        }
    }
}
