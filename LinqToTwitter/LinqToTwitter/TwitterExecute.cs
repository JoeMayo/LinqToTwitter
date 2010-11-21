using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
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
        #region Properties

        /// <summary>
        /// Version used in UserAgent
        /// </summary>
        private const string m_linqToTwitterVersion = "LINQ to Twitter v2.0";

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

        ///// <summary>
        ///// Gets or sets the object that can send authorized requests to Twitter.
        ///// </summary>
        //public ITwitterAuthorization AuthorizedClient { get; set; }

        /// <summary>
        /// Gets or sets the object that can send authorized requests to Twitter.
        /// </summary>
        public ITwitterAuthorizer AuthorizedClient { get; set; }

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
        /// Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        /// This is very useful for debugging
        /// </remarks>
        public string LastUrl { get; private set; }

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

        private static TextWriter log;
        public static TextWriter Log
        {
            get { return log; }
            set { log = value; }
        }

        /// <summary>
        /// Allows users to process content returned from stream
        /// </summary>
        public Action<StreamContent> StreamingCallback { get; set; }

        private static object m_closeStreamLock = new object();
        private bool m_closeStream;

        /// <summary>
        /// Set to true to close stream, false means stream is still open
        /// </summary>
        public bool CloseStream
        {
            get
            {
                lock (m_closeStreamLock)
                {
                    return m_closeStream;
                }
            }
            set
            {
                lock (m_closeStreamLock)
                {
                    m_closeStream = value;
                }
            }
        }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingUserName { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingPassword { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Used to notify callers of changes in image upload progress
        /// </summary>
        public event EventHandler<TwitterProgressEventArgs> UploadProgressChanged;

        /// <summary>
        /// Call this to notify users of percentage of completion of operation.
        /// </summary>
        /// <param name="percent">Percent complete.</param>
        private void OnUploadProgressChanged(int percent)
        {
            if (UploadProgressChanged != null)
            {
                var progressEventArgs = new TwitterProgressEventArgs
                {
                    PercentComplete = percent
                };
                UploadProgressChanged(this, progressEventArgs);
            }
        }

        #endregion

        #region Initialization

        ///// <summary>
        ///// supports testing
        ///// </summary>
        ///// <param name="oAuthTwitter">IOAuthTwitter Mock</param>
        //public TwitterExecute(ITwitterAuthorization authorizedClient)
        //{
        //    if (authorizedClient == null)
        //    {
        //        throw new ArgumentNullException("authorizedClient");
        //    }

        //    this.AuthorizedClient = authorizedClient;
        //    this.AuthorizedClient.UserAgent = m_linqToTwitterVersion;
        //}

        /// <summary>
        /// supports testing
        /// </summary>
        /// <param name="oAuthTwitter">IOAuthTwitter Mock</param>
        public TwitterExecute(ITwitterAuthorizer authorizedClient)
        {
            if (authorizedClient == null)
            {
                throw new ArgumentNullException("authorizedClient");
            }

            this.AuthorizedClient = authorizedClient;
            this.AuthorizedClient.UserAgent = m_linqToTwitterVersion;
        }

        #endregion

        #region Exception and Response Handling

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
            string responseBody;

            using (var respStream = resp.GetResponseStream())
            using (var respReader = new StreamReader(respStream))
            {
                responseBody = respReader.ReadToEnd();
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

            return responseBody;
        }

        /// <summary>
        /// Throws exception if error returned from Twitter
        /// </summary>
        /// <param name="responseStr">XML string response from Twitter</param>
        /// <param name="status">HTTP Error number</param>
        private void CheckResultsForTwitterError(string responseStr, string status)
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
        }

        #endregion

        #region Execution

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>XML Respose from Twitter</returns>
        public string QueryTwitter(string url)
        {
            //Log
            WriteLog(url, "QueryTwitter");

            var uri = new Uri(url);
            string responseXml = string.Empty;
            string httpStatus = string.Empty;

            try
            {
                this.LastUrl = uri.AbsoluteUri;
                var req = this.AuthorizedClient.Get(url);

                var resetEvent = new ManualResetEvent(initialState: false);
                HttpWebResponse res = null;

                req.BeginGetResponse(
                    new AsyncCallback(
                        ar =>
                        {
                            res = req.EndGetResponse(ar) as HttpWebResponse;
                            httpStatus = res.Headers["Status"];
                            responseXml = GetTwitterResponse(res);
                            resetEvent.Set();
                        }), null);

                resetEvent.WaitOne();
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            if (uri.LocalPath.EndsWith("json"))
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXml));
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);

                var doc = XDocument.Load(reader);
                responseXml = doc.ToString();
            }

            CheckResultsForTwitterError(responseXml, httpStatus);

            return responseXml;
        }

        /// <summary>
        /// Selects appropriate streaming method
        /// </summary>
        /// <param name="url">Stream url</param>
        /// <returns>
        /// Caller expects an XML formatted string response, but
        /// real response(s) with streams is fed to the callback
        /// </returns>
        public string QueryTwitterStream(string url)
        {
            if (url.Contains("user.json") || url.Contains("site.json"))
            {
                new Thread(ManageTwitterUserStream).Start(url);
            }
            else
            {
                new Thread(ManageTwitterStreaming).Start(url); 
            }
            return "<streaming></streaming>";
        }

        /// <summary>
        /// This code will execute on a thread, processing content from the Twitter stream
        /// </summary>
        /// <remarks>
        /// Values are returned to invocations of StreamingCallback. Remember that these callbacks
        /// are running on a separate thread and it is the caller's responsibility to marshal back
        /// onto UI thread, if applicable.
        /// 
        /// Thanks to Shannon Whitley for a good example of how to do this in C#:
        /// http://www.voiceoftech.com/swhitley/?p=898
        /// </remarks>
        /// <param name="req">Web request, which has already been authenticated</param>
        private void ManageTwitterStreaming(object url)
        {
            string streamUrl = url as string;

            var req = HttpWebRequest.Create(streamUrl) as HttpWebRequest;
            req.Credentials = new NetworkCredential(StreamingUserName, StreamingPassword);
            req.UserAgent = UserAgent;
            //req.Timeout = -1;

            byte[] bytes = new byte[0];

            bool shouldPostQuery = streamUrl.Contains("filter.json");

            if (shouldPostQuery)
            {
                int qIndex = streamUrl.IndexOf('?');
                string urlParams = streamUrl.Substring(qIndex);
                streamUrl = streamUrl.Substring(qIndex - 1);

                bytes = Encoding.UTF8.GetBytes(urlParams);
                req.ContentLength = bytes.Length;
                req.Method = "POST";
                req.ContentType = "x-www-form-urlencoded";
                //req.ServicePoint.Expect100Continue = false;
            }

            ExecuteTwitterStream(req, bytes, shouldPostQuery);
        }

        /// <summary>
        /// This code will execute on a thread, processing content from the Twitter user stream
        /// </summary>
        /// <remarks>
        /// Values are returned to invocations of StreamingCallback. Remember that these callbacks
        /// are running on a separate thread and it is the caller's responsibility to marshal back
        /// onto UI thread, if applicable.
        /// 
        /// Thanks to Shannon Whitley for a good example of how to do this in C#:
        /// http://www.voiceoftech.com/swhitley/?p=898
        /// </remarks>
        /// <param name="req">Web request, which has already been authenticated</param>
        private void ManageTwitterUserStream(object url)
        {
            string streamUrl = url as string;

            var uri = new Uri(streamUrl);
            string responseXml = string.Empty;
            string httpStatus = string.Empty;

            this.LastUrl = uri.AbsoluteUri;
            var req = this.AuthorizedClient.Get(streamUrl);
            req.UserAgent = UserAgent;
            //req.Timeout = -1;

            ExecuteTwitterStream(req, null, shouldPostQuery: false);
        }

        /// <summary>
        /// Processes stream results and performs error handling
        /// </summary>
        /// <param name="req">HTTP Request</param>
        /// <param name="postBytes">Contains post data if shouldPostQuery is true</param>
        /// <param name="shouldPostQuery">If true, send postBytes as post</param>
        private void ExecuteTwitterStream(HttpWebRequest req, byte[] postBytes, bool shouldPostQuery)
        {
            var resetEvent = new ManualResetEvent(initialState: false);
            int errorWait = 250;
            bool firstConnection = true;

            try
            {
                while (!CloseStream)
                {
                    if (shouldPostQuery)
                    {
                        req.BeginGetRequestStream(
                            new AsyncCallback(
                                ar =>
                                {
                                    using (var requestStream = req.EndGetRequestStream(ar))
                                    {
                                        requestStream.Write(postBytes, 0, postBytes.Length);
                                    }

                                    resetEvent.Set();

                                }), null);

                        resetEvent.WaitOne();
                        resetEvent.Reset();
                    }

                    req.BeginGetResponse(
                        new AsyncCallback(ar =>
                        {
                            HttpWebResponse resp = null;

                            try
                            {
                                resp = req.EndGetResponse(ar) as HttpWebResponse;

                                using (var stream = resp.GetResponseStream())
                                using (var respRdr = new StreamReader(stream, Encoding.UTF8))
                                {
                                    string content = null;

                                    try
                                    {
                                        do
                                        {
                                            content = respRdr.ReadLine();

                                            // launch on a separate thread to keep user's 
                                            // callback code from blocking the stream.
                                            new Thread(InvokeStreamCallback).Start(content);

                                            errorWait = 250;
                                        }
                                        while (!CloseStream);
                                    }
                                    catch (WebException wex)
                                    {
                                        // TODO: refactor errorWait caculation into separate method and verify with http://dev.twitter.com/pages/user_streams_suggestions#Startup
                                        if (wex.Status == WebExceptionStatus.ProtocolError)
                                        {
                                            if (errorWait < 10000)
                                            {
                                                errorWait = 10000;
                                            }
                                            else
                                            {
                                                if (errorWait < 240000)
                                                {
                                                    errorWait *= 2;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (errorWait < 16000)
                                            {
                                                errorWait += 250;
                                            }
                                        }

                                        WriteLog(wex.ToString() + ", Waiting " + errorWait + " seconds.  ", "ExecuteStream");
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog(ex.ToString(), "ExecuteStream");
                                    }
                                    finally
                                    {
                                        if (req != null)
                                        {
                                            req.Abort();
                                        }

                                        Thread.Sleep(errorWait);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // TODO: Needs more work: http://dev.twitter.com/pages/user_streams_suggestions#StreamingConnectionFailure
                                if (firstConnection)
                                {
                                    firstConnection = false;
                                    errorWait = 0;
                                }
                                else
                                {
                                    errorWait += 250;
                                }
                                WriteLog(ex.ToString(), "ExecuteStream");
                            }
                            finally
                            {
                                if (req != null)
                                {
                                    req.Abort();
                                }

                                Thread.Sleep(errorWait);
                            }

                            resetEvent.Set();

                        }), null);


                    resetEvent.WaitOne();
                    resetEvent.Reset();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString(), "ManageTwitterStream");
                Thread.Sleep(errorWait);
                throw;
            }
        }

        /// <summary>
        /// Executes callback handler
        /// </summary>
        /// <remarks>
        /// If the user's callback code fails to handle an exception
        /// this code will log and re-throw.  The user should consider
        /// ensuring the code they write doesn't do anything
        /// that will get them rate-limited or black-listed on Twitter.
        /// </remarks>
        /// <param name="content">Content from Twitter</param>
        private void InvokeStreamCallback(object content)
        {
            try
            {
                StreamingCallback(new StreamContent(this, content as string));
            }
            catch (Exception ex)
            {
                WriteLog("Unhandled exception in your StreamingCallback code.  " + ex.ToString(), "InvokeCallback");
                throw;
            }
        }

        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="filePath">full path of file to upload</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="url">url to upload to</param>
        /// <returns>XML Respose from Twitter</returns>
        public string PostTwitterFile(string filePath, Dictionary<string, string> parameters, string url)
        {
            var fileName = Path.GetFileName(filePath);

            string imageType;

            switch (Path.GetExtension(fileName).ToLower())
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

            byte[] fileBytes = Utilities.GetFileBytes(filePath);

            return PostTwitterImage(fileBytes, parameters, url, fileName, imageType);
        }

        /// <summary>
        /// performs HTTP POST image byte array upload to Twitter
        /// </summary>
        /// <param name="image">byte array containing image to upload</param>
        /// <param name="url">url to upload to</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <returns>XML Response from Twitter</returns>
        public string PostTwitterImage(byte[] image, Dictionary<string, string> parameters, string url, string fileName, string imageType)
        {
            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
            var contentDisposition = string.Format("Content-Disposition:form-data); name=\"image\"); filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", fileName, imageType);
            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

            var formDataSB = new StringBuilder();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var param in parameters)
                {
                    formDataSB.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", contentBoundaryBase, param.Key, param.Value);
                }
            }

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string imageByteString = encoding.GetString(image, 0, image.Length);

            byte[] imageBytes =
                encoding.GetBytes(
                    formDataSB.ToString() +
                    beginContentBoundary +
                    contentDisposition +
                    imageByteString +
                    endContentBoundary);

            string responseXml = string.Empty;
            string httpStatus = string.Empty;

            try
            {
                this.LastUrl = url;
                //Log
                WriteLog(url, "PostTwitterImage");

                var req = this.AuthorizedClient.Post(url);
                //req.ServicePoint.Expect100Continue = false;
                req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
                //req.PreAuthenticate = true;
                req.AllowWriteStreamBuffering = true;
                req.ContentLength = imageBytes.Length;

                var resetEvent = new ManualResetEvent(initialState: false);

                req.BeginGetRequestStream(
                    new AsyncCallback(
                        ar =>
                        {
                            using (var reqStream = req.EndGetRequestStream(ar))
                            {
                                int offset = 0;
                                int bufferSize = 4096;
                                int lastPercentage = 0;
                                while (offset < imageBytes.Length)
                                {
                                    int bytesLeft = imageBytes.Length - offset;

                                    if (bytesLeft < bufferSize)
                                    {
                                        reqStream.Write(imageBytes, offset, bytesLeft);
                                    }
                                    else
                                    {
                                        reqStream.Write(imageBytes, offset, bufferSize);
                                    }

                                    offset += bufferSize;

                                    int percentComplete =
                                        (int)((double)offset / (double)imageBytes.Length * 100);

                                    // since we still need to get the response later
                                    // in the algorithm, interpolate the results to
                                    // give user a more accurate picture of completion.
                                    // i.e. we don't want to shoot up to 100% here when
                                    // we know there is more processing to do.
                                    lastPercentage = percentComplete >= 98 ?
                                        100 - ((98 - lastPercentage) / 2) :
                                        percentComplete;

                                    OnUploadProgressChanged(lastPercentage);
                                }

                                reqStream.Flush();
                            }
                            resetEvent.Set();
                        }), null);

                resetEvent.WaitOne();
                resetEvent.Reset();

                req.BeginGetResponse(
                    new AsyncCallback(
                        ar =>
                        {
                            using (var res = req.EndGetResponse(ar) as HttpWebResponse)
                            {
                                httpStatus = res.Headers["Status"];
                                responseXml = GetTwitterResponse(res);
                                OnUploadProgressChanged(99);
                            }
                            resetEvent.Set();
                        }), null);

                resetEvent.WaitOne();
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            OnUploadProgressChanged(100);
            CheckResultsForTwitterError(responseXml, httpStatus);
            return responseXml;
        }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <returns>XML response from Twitter</returns>
        public string ExecuteTwitter(string url, Dictionary<string, string> parameters)
        {
            string httpStatus = string.Empty;
            string responseXml = string.Empty;

            // Oddly, we add the parameters both to the URI's query string and the POST entity
            Uri requestUri = Utilities.AppendQueryString(new Uri(url), parameters);
            try
            {
                this.LastUrl = requestUri.AbsoluteUri;
                //Log
                WriteLog(url, "ExecuteTwitter");

                using (var resp = this.AuthorizedClient.Post(url, parameters))
                {
                    httpStatus = resp.Headers["Status"];
                    responseXml = GetTwitterResponse(resp);
                }
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            CheckResultsForTwitterError(responseXml, httpStatus);
            return responseXml;
        }

        private void WriteLog(string content, string currentMethod)
        {
            if (log != null)
            {
                log.Flush();
                log.WriteLine("--Log Starts Here--");
                log.WriteLine("Query:" + content);
                log.WriteLine("Method:" + currentMethod);
                log.WriteLine("--Log Ends Here--");
                log.Close();
            }
        }

        #endregion

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
