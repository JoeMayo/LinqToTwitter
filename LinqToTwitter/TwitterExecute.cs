using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Makes LINQ to Twitter more testable by isolating 
    /// execution routines that communicate with Twitter 
    /// from the rest of the logic.
    /// </summary>
    [Serializable]
    internal class TwitterExecute : ITwitterExecute, IDisposable
    {
        /// <summary>
        /// Version used in UserAgent
        /// </summary>
        private const string m_linqToTwitterVersion = "LINQ to Twitter v2.0";

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

        /// <summary>
        /// Gets or sets the object that can send authorized requests to Twitter.
        /// </summary>
        public ITwitterAuthorization AuthorizedClient { get; set; }

        /// <summary>
        /// Timeout (milliseconds) for writing to request 
        /// stream or reading from response stream
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return (int)this.AuthorizedClient.ReadWriteTimeout.TotalMilliseconds; }
            set { this.AuthorizedClient.ReadWriteTimeout = TimeSpan.FromMilliseconds(value); }
        }

        /// <summary>
        /// Default for Timeout
        /// </summary>
        public const int DefaultTimeout = 100000;

        /// <summary>
        /// Timeout (milliseconds) to wait for a server response
        /// </summary>
        public int Timeout
        {
            get { return (int)this.AuthorizedClient.Timeout.TotalMilliseconds; }
            set { this.AuthorizedClient.Timeout = TimeSpan.FromMilliseconds(value); }
        }

        /// <summary>
        /// list of response headers from query
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                return this.AuthorizedClient.UserAgent;
            }
            set
            {
                this.AuthorizedClient.UserAgent =
                    string.IsNullOrEmpty(value) ?
                        this.AuthorizedClient.UserAgent :
                        value + ";" + this.AuthorizedClient.UserAgent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterExecute"/> class.
        /// </summary>
        public TwitterExecute()
        {
            this.AuthorizedClient = new UsernamePasswordAuthorization();
            this.AuthorizedClient.UserAgent = m_linqToTwitterVersion;
        }

        /// <summary>
        /// supports testing
        /// </summary>
        /// <param name="oAuthTwitter">IOAuthTwitter Mock</param>
        public TwitterExecute(ITwitterAuthorization authorizedClient)
        {
            if (authorizedClient == null)
            {
                throw new ArgumentNullException("authorizedClient");
            }

            this.AuthorizedClient = authorizedClient;
            this.AuthorizedClient.UserAgent = m_linqToTwitterVersion;
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
                    Request = responseXml.Element("request") == null ? "request URI not received from Twitter" : responseXml.Element("request").Value,
                    Error = responseXml.Element("error") == null ? "error message not received from Twitter" : responseXml.Element("error").Value
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

            if (responseXml.Name == "hash" &&
                responseXml.Element("error") != null)
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
            var req = this.AuthorizedClient.Get(new Uri(url), null);

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

            var req = this.AuthorizedClient.Post(new Uri(url));
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
            req.PreAuthenticate = true;
            req.AllowWriteStreamBuffering = true;
            req.ContentLength = fileBytes.Length;

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
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        public IList ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            string httpStatus = string.Empty;
            string responseXml = string.Empty;

            // Oddly, we add the parameters both to the URI's query string and the POST entity
            Uri requestUri = Utilities.AppendQueryString(new Uri(url), parameters);
            using (var resp = this.AuthorizedClient.Post(requestUri, parameters))
            {
                try
                {
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
            }

            return ProcessResults(requestProcessor, responseXml, httpStatus);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableClient = this.AuthorizedClient as IDisposable;
                if (disposableClient != null)
                {
                    disposableClient.Dispose();
                }
            }
        }

        #endregion
    }
}
