using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;

using LinqToTwitter.Common;
using LitJson;

#if NETFX_CORE
using System.Threading.Tasks;
#endif

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE && !L2T_PCL
#endif

#if !SILVERLIGHT && !L2T_PCL
using System.IO.Compression;
#else
using Ionic.Zlib;
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// Logic that performs actual communication with Twitter
    /// </summary>
    internal class TwitterExecute : ITwitterExecute, IDisposable
    {
        /// <summary>
        /// Version used in UserAgent
        /// </summary>
        const string LinqToTwitterVersion = "LINQ to Twitter v2.0";

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

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
            get { return (int)AuthorizedClient.ReadWriteTimeout.TotalMilliseconds; }
            set { AuthorizedClient.ReadWriteTimeout = TimeSpan.FromMilliseconds(value); }
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
            get { return (int)AuthorizedClient.Timeout.TotalMilliseconds; }
            set { AuthorizedClient.Timeout = TimeSpan.FromMilliseconds(value); }
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
                return AuthorizedClient.UserAgent;
            }
            set
            {
                AuthorizedClient.UserAgent =
                    string.IsNullOrEmpty(value) ?
                        AuthorizedClient.UserAgent :
                        value + ";" + AuthorizedClient.UserAgent;
            }
        }

        /// <summary>
        /// Assign your TextWriter instance to receive LINQ to Twitter output
        /// </summary>
        public static TextWriter Log { get; set; }

        readonly object streamingCallbackLock = new object();

        /// <summary>
        /// Allows users to process content returned from stream
        /// </summary>
        public Action<StreamContent> StreamingCallback { get; set; }

        /// <summary>
        /// Set to true to close stream, false means stream is still open
        /// </summary>
        public bool CloseStream { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingUserName { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingPassword { get; set; }

        readonly object asyncCallbackLock = new object();

        /// <summary>
        /// Allows users to process content returned from stream
        /// </summary>
        public Delegate AsyncCallback { get; set; }

        /// <summary>
        /// Used to notify callers of changes in image upload progress
        /// </summary>
        public event EventHandler<TwitterProgressEventArgs> UploadProgressChanged;

        /// <summary>
        /// Call this to notify users of percentage of completion of operation.
        /// </summary>
        /// <param name="percent">Percent complete.</param>
        void OnUploadProgressChanged(int percent)
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

        /// <summary>
        /// supports testing
        /// </summary>
        public TwitterExecute(ITwitterAuthorizer authorizedClient)
        {
            if (authorizedClient == null)
            {
                throw new ArgumentNullException("authorizedClient");
            }

            AuthorizedClient = authorizedClient;
            AuthorizedClient.UserAgent = LinqToTwitterVersion;
        }

        /// <summary>
        /// Common code to construct a TwitterQueryException instance
        /// </summary>
        /// <param name="responseStr">Response from Twitter</param>
        /// <param name="wex">WebException assigned as InnerException if available</param>
        /// <returns>An Instance of TwitterQueryException</returns>
        TwitterQueryException ConstructTwitterQueryException(string responseStr, WebException wex)
        {
            JsonData responseJson = JsonMapper.ToObject(responseStr);

            TwitterQueryException twitterQueryEx = null;

            var errors = responseJson.GetValue<JsonData>("errors");
            if (errors != null && errors.Count > 0)
            {
                var error = errors[0];
                twitterQueryEx = new TwitterQueryException(error.GetValue<string>("message"), wex)
                {
                    HttpError = wex == null ? string.Empty : wex.Status.ToString(),
                    ErrorCode = error.GetValue<int>("code")
                };
            }
            else
            {
                twitterQueryEx = new TwitterQueryException("Error while querying Twitter.", wex);
            }

            return twitterQueryEx;
        }

        /// <summary>
        /// Throws exception if error returned from Twitter
        /// </summary>
        /// <param name="responseStr">XML or JSON string response from Twitter</param>
        /// <param name="status">HTTP Error number</param>
        internal void CheckResultsForTwitterError(string responseStr, string status)
        {
            if (responseStr.StartsWith("{", StringComparison.Ordinal))
            {
                TwitterQueryException twitterQueryEx = ConstructTwitterQueryException(responseStr, null);

                if (twitterQueryEx.ErrorCode != 0)
                    throw twitterQueryEx;
            }
        }

        /// <summary>
        /// generates a new TwitterQueryException from a WebException
        /// </summary>
        /// <param name="wex">Web Exception to Translate</param>
        /// <returns>new TwitterQueryException instance</returns>
        internal TwitterQueryException CreateTwitterQueryException(WebException wex)
        {
            const string DefaultResponse = @"{""errors"":[{""message"":""No message from Twitter"",""code"":0}]}";
            string responseStr = DefaultResponse;

            try
            {
                if (wex != null && wex.Response != null)
                {
                    responseStr = GetTwitterResponse(wex.Response);
                }
            }
            catch (Exception)
            {
                responseStr = DefaultResponse;
            }

            if (!responseStr.StartsWith("{", StringComparison.Ordinal))
                responseStr = DefaultResponse;

            TwitterQueryException twitterQueryEx = ConstructTwitterQueryException(responseStr, wex);

            return twitterQueryEx;
        }

        string ReadStreamBytes(Stream stream)
        {
            const int ByteCount = 4096;
            var sb = new StringBuilder();

            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    var buffer = new char[ByteCount];
                    reader.ReadBlock(buffer, 0, ByteCount);
                    sb.Append(buffer);
                }
            }

            return sb.ToString().Trim('\0');
        }

        /// <summary>
        /// gets WebResponse contents from Twitter
        /// </summary>
        /// <param name="resp">WebResponse to extract string from</param>
        /// <returns>XML string response from Twitter</returns>
        string GetTwitterResponse(WebResponse resp)
        {
            string responseBody;

            using (var respStream = resp.GetResponseStream())
            {
                string contentEncoding = string.Empty;
                
#if !SILVERLIGHT
                contentEncoding = resp.Headers["Content-Encoding"] ?? "";
#endif

                if (contentEncoding.ToLower().Contains("gzip"))
                {
                    using (var gzip = new GZipStream(respStream, CompressionMode.Decompress))
                    {
                        responseBody = ReadStreamBytes(gzip);
                    }
                }
                else if (contentEncoding.ToLower().Contains("deflate"))
                {
                    using (var dflate = new DeflateStream(respStream, CompressionMode.Decompress))
                    {
                        responseBody = ReadStreamBytes(dflate);
                    }
                }
                else if (resp.ContentType.StartsWith("image"))
                {
                    responseBody = "{ \"imageUrl\": \"" + resp.ResponseUri.ToString() + "\" }";
                }
                else
                {
                    responseBody = ReadStreamBytes(respStream);
                }
            }

            var responseHeaders = new Dictionary<string, string>();

#if !SILVERLIGHT
            foreach (string key in resp.Headers.AllKeys)
            {
                responseHeaders.Add(key, resp.Headers[key].ToString());
            }
#endif

            ResponseHeaders = responseHeaders;

            return responseBody;
        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Respose from Twitter</returns>
        public string QueryTwitter<T>(Request request, IRequestProcessor<T> reqProc)
        {
            //Log
            var url = request.Endpoint;
            var parameters = request.RequestParameters;
            LastUrl = request.FullUrl;
            WriteLog(LastUrl, "QueryTwitter");

            var uri = new Uri(LastUrl);
            string response = string.Empty;
            string httpStatus = string.Empty;
            Exception thrownException = null;

            try
            {
                var req = AuthorizedClient.Get(request);
#if !SILVERLIGHT
                bool initialStateSignaled = AsyncCallback != null;

                using (var resetEvent = new ManualResetEvent(initialStateSignaled))
                {
#endif
                    req.BeginGetResponse(
                        new AsyncCallback(
                            ar =>
                            {
                                lock (asyncCallbackLock)
                                {
                                    var asyncResp = new TwitterAsyncResponse<IEnumerable<T>>();
                                    try
                                    {
                                        var res = req.EndGetResponse(ar) as HttpWebResponse;
                                        httpStatus = (int)res.StatusCode + " " + res.StatusDescription;
                                        response = GetTwitterResponse(res);

                                        if (AsyncCallback != null)
                                            asyncResp.State = reqProc.ProcessResults(response);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (AsyncCallback == null)
                                            thrownException = ex;

                                        asyncResp.Status = TwitterErrorStatus.RequestProcessingException;
                                        asyncResp.Message = "Processing failed. See Error property for more details.";
                                        asyncResp.Error = ex;
                                    }
                                    finally
                                    {
                                        if (AsyncCallback != null)
                                        {
                                            if (AsyncCallback is Action<IEnumerable<T>>)
                                                (AsyncCallback as Action<IEnumerable<T>>)(asyncResp.State);
                                            else
                                                (AsyncCallback as Action<TwitterAsyncResponse<IEnumerable<T>>>)(asyncResp);

                                            AsyncCallback = null;
                                        }
#if !SILVERLIGHT
                                        else
                                            resetEvent.Set();
#endif
                                    }
                                }
                            }), null);
#if !SILVERLIGHT
                    if (AsyncCallback == null)
                    {
                        resetEvent.WaitOne();

                        if (thrownException != null)
                            throw thrownException;
                    }
                }
#endif
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            CheckResultsForTwitterError(response, httpStatus);

            return response;
        }

        /// <summary>
        /// Performs a query on the Twitter Stream
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <returns>
        /// Caller expects an XML formatted string response, but
        /// real response(s) with streams is fed to the callback
        /// </returns>
        public string QueryTwitterStream(Request request)
        {
#if NETFX_CORE
            Task.Run(() => ExecuteTwitterStream(request));
#else
            ThreadPool.QueueUserWorkItem(ExecuteTwitterStream, request);
#endif
            return "<streaming></streaming>";
        }

        /// <summary>
        /// Processes stream results and performs error handling
        /// </summary>
        /// <param name="state">The request</param>
        void ExecuteTwitterStream(object state)
        {
            var request = state as Request;
            Debug.Assert(request != null, "state must be a Request object");

            var streamUrl = request.Endpoint;

            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
            {
                int errorWait = 250;
                bool firstConnection = true;

                try
                {
                    HttpWebRequest req = null;

                    while (!CloseStream)
                    {
                        if (streamUrl.Contains("user.json") || streamUrl.Contains("site.json"))
                        {
                            req = GetUserStreamRequest(request);
                        }
                        else
                        {
                            req = GetBasicStreamRequest(request);
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
                                        firstConnection = true;
                                        string content = null;

                                        try
                                        {
                                            do
                                            {
#if !SILVERLIGHT && !NETFX_CORE
                                                try
                                                {
#endif
                                                    lock (streamingCallbackLock)
                                                    {
                                                        content = respRdr.ReadLine();

                                                        // When Twitter breaks the connection, we need to exit the
                                                        // entire loop and start over. Otherwise, the readlines
                                                        // keep returning blank lines that are incorrectly interpreted
                                                        // as keep-alive messages in a tight loop.
                                                        if (respRdr.EndOfStream)
                                                        {
                                                            CloseStream = true;
                                                            throw new WebException("Twitter closed the stream.", WebExceptionStatus.ConnectFailure);
                                                        }

#if NETFX_CORE
                                                        Task.Run(() => InvokeStreamCallback(content));
#else
                                                        ThreadPool.QueueUserWorkItem(InvokeStreamCallback, content);
#endif
                                                    }

                                                    errorWait = 250;
#if !SILVERLIGHT && !NETFX_CORE
                                                }
                                                catch (WebException wex)
                                                {
                                                    // Timeouts are expected, as set by ReadWriteTimeout
                                                    // on respRdr.BaseStream.ReadTimeout
                                                    if (wex.Status != WebExceptionStatus.Timeout)
                                                        throw;
                                                }
#endif
                                            }
                                            while (!CloseStream);
                                        }
                                        catch (WebException wex)
                                        {
                                            switch (wex.Status)
                                            {
                                                case WebExceptionStatus.Success:
                                                    break;
                                                case WebExceptionStatus.ConnectFailure:
                                                    // Twitter closed the connection, so keep backing out
                                                    throw;
                                                case WebExceptionStatus.MessageLengthLimitExceeded:
                                                case WebExceptionStatus.Pending:
                                                case WebExceptionStatus.RequestCanceled:
                                                case WebExceptionStatus.SendFailure:
                                                case WebExceptionStatus.UnknownError:
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

                                                    WriteLog(wex.ToString() + ", Waiting " + errorWait / 1000 + " seconds.  ", "ExecuteStream");
                                                    break;
                                                default:
                                                    if (errorWait < 16000)
                                                    {
                                                        errorWait += 250;
                                                    }
                                                    break;
                                            }
#if NETFX_CORE
                                            Task.Delay(errorWait);
#else
                                            Thread.Sleep(errorWait);
#endif
                                        }
                                        finally
                                        {
                                            if (req != null)
                                            {
                                                req.Abort();
                                            }
                                        }
                                    }
                                }
                                catch (WebException ex)
                                {
                                    WriteLog(ex.ToString(), "ExecuteTwitterStream");

                                    // Twitter closed the connection, so send the appropriate
                                    // exception, WebException with ConnectionFailed status to
                                    // the user's callback so they can decide whether to reconnect.
#if NETFX_CORE
                                    Task.Run(() => InvokeStreamCallback(ex));
#else
                                    ThreadPool.QueueUserWorkItem(InvokeStreamCallback, ex);
#endif
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    if (firstConnection)
                                    {
                                        firstConnection = false;
                                        errorWait = new Random().Next(20000, 40000);
                                    }
                                    else
                                    {
                                        if (errorWait < 300000)
                                        {
                                            errorWait *= 2;
                                        }
                                    }
                                    WriteLog(ex.ToString() + ", Waiting " + errorWait / 1000 + " seconds.  ", "ExecuteStream");
#if NETFX_CORE
                                    Task.Delay(errorWait);
#else
                                    Thread.Sleep(errorWait);
#endif
                                }
                                finally
                                {
                                    if (req != null)
                                    {
                                        req.Abort();
                                    }

                                    if (resetEvent != null) resetEvent.Set();
                                }
                            }), null);

                        resetEvent.WaitOne();
                        resetEvent.Reset();
                    }
                }
                finally
                {
                    // user might want to try to reconnect with
                    // the same instance of TwitterContext
                    CloseStream = false;
                }
            }
        }

        /// <summary>
        /// Handles request initialization for sample, filter, and other basic streams
        /// </summary>
        /// <param name="request">Stream endpoint and parameters</param>
        /// <returns>Initialized Request</returns>
        HttpWebRequest GetBasicStreamRequest(Request request)
        {
            HttpWebRequest req = null;
            byte[] bytes = new byte[0];
            LastUrl = request.FullUrl;
            bool shouldPostQuery = LastUrl.Contains("filter.json");

            if (shouldPostQuery)
            {
                int qIndex = LastUrl.IndexOf('?');

                string urlParams = LastUrl.Substring(qIndex + 1);
                bytes = Encoding.UTF8.GetBytes(urlParams);

                req = WebRequest.Create(LastUrl) as HttpWebRequest;

                req.Method = "POST";
                req.ContentType = "x-www-form-urlencoded";

#if !WINDOWS_PHONE && !NETFX_CORE
                req.ContentLength = bytes.Length;
#endif

#if !SILVERLIGHT && !NETFX_CORE
                if (Timeout > 0)
                    req.Timeout = Timeout;

                if (ReadWriteTimeout > 0)
                    req.ReadWriteTimeout = ReadWriteTimeout;
#endif
#if WINDOWS_PHONE
                req.AllowReadStreamBuffering = false;
#endif
                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
                {
                    Exception asyncException = null;

                    req.BeginGetRequestStream(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    using (var requestStream = req.EndGetRequestStream(ar))
                                    {
                                        requestStream.Write(bytes, 0, bytes.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                    WriteLog(ex.ToString(), "GetBasicStreamRequest");
                                    throw;
                                }

                                resetEvent.Set();

                            }), null);

                    resetEvent.WaitOne();

                    if (asyncException != null)
                        throw asyncException;
                }
            }
            else
            {
                req = WebRequest.Create(LastUrl) as HttpWebRequest;
            }

            req.Credentials = new NetworkCredential(StreamingUserName, StreamingPassword);

#if !SILVERLIGHT && !NETFX_CORE
            req.UserAgent = UserAgent;
#endif

            return req;
        }

        /// <summary>
        /// Handles request initialization for user and site streams
        /// </summary>
        /// <param name="request">Stream endpoint and parameters</param>
        /// <returns>Initialized Request</returns>
        HttpWebRequest GetUserStreamRequest(Request request)
        {
            this.LastUrl = request.FullUrl;
            var req = this.AuthorizedClient.Get(request) as HttpWebRequest;
#if !SILVERLIGHT && !NETFX_CORE
            req.UserAgent = UserAgent;
#endif
#if WINDOWS_PHONE
            req.AllowReadStreamBuffering = false;
#endif

            return req;
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
        void InvokeStreamCallback(object content)
        {
            try
            {
                StreamContent strmContent = null;

                if (content is string)
                {
                    strmContent = new StreamContent(this, content as string); 
                }
                else
                {
                    const string Message =
                        "An error has occurred during processing. " +
                        "This is sometimes an unknown error. " +
                        "It can also happen when Twitter closes the stream. " +
                        "Whatever the cause, your stream has been closed. " +
                        "You can find more information about this exception in the 'Error' property.";

                    strmContent = new StreamContent(this, Message);
                    strmContent.Status = TwitterErrorStatus.RequestProcessingException;
                    strmContent.Error = content as Exception;
                }

                StreamingCallback(strmContent);
            }
            catch (Exception ex)
            {
                WriteLog("Unhandled exception in your StreamingCallback code.  " + ex, "InvokeCallback");
                throw;
            }
        }

#if !NETFX_CORE
        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="url">url to upload to</param>
        /// <param name="postData">query string parameters</param>
        /// <param name="filePath">full path of file to upload</param>
        /// <param name="reqProc">Processes results of async requests</param>
        /// <returns>XML Respose from Twitter</returns>
        public string PostTwitterFile<T>(string url, IDictionary<string, string> postData, string filePath, IRequestProcessor<T> reqProc)
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

            return PostTwitterImage(url, postData, fileBytes, fileName, imageType, reqProc);
        }
#endif

        /// <summary>
        /// performs HTTP POST image byte array upload to Twitter
        /// </summary>
        /// <param name="url">url to upload to</param>
        /// <param name="postData">parameters to pass</param>
        /// <param name="image">byte array containing image to upload</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="reqProc">Processes results of async requests</param>
        /// <returns>XML Response from Twitter</returns>
        public string PostTwitterImage<T>(string url, IDictionary<string, string> postData, byte[] image, string fileName, string imageType, IRequestProcessor<T> reqProc)
        {
            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
            var contentDisposition = string.Format("Content-Disposition:form-data); name=\"image\"); filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", fileName, imageType);
            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

            var formDataSb = new StringBuilder();

            if (postData != null && postData.Count > 0)
            {
                foreach (var param in postData)
                {
                    if (param.Value == "IMAGE_DATA")
                    {
                        contentDisposition = contentDisposition.Replace("name=\"image\"", "name=\"" + param.Key + "\"");
                    }
                    else
                    {
                        formDataSb.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", contentBoundaryBase, param.Key, param.Value);
                    }
                }
            }

            var encoding = Encoding.GetEncoding("iso-8859-1");
            string imageByteString = encoding.GetString(image, 0, image.Length);

            byte[] imageBytes =
                encoding.GetBytes(
                    formDataSb.ToString() +
                    beginContentBoundary +
                    contentDisposition +
                    imageByteString +
                    endContentBoundary);

            string response = string.Empty;
            string httpStatus = string.Empty;

            try
            {
                LastUrl = url;
                //Log
                WriteLog(LastUrl, "PostTwitterImage");

                var req = AuthorizedClient.PostRequest(new Request(url), new Dictionary<string, string>());
                req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;

#if !WINDOWS_PHONE && !NETFX_CORE
                req.AllowWriteStreamBuffering = true;
                req.ContentLength = imageBytes.Length; 
#endif

                Exception asyncException = null;
                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
                {

                    req.BeginGetRequestStream(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    using (var reqStream = req.EndGetRequestStream(ar))
                                    {
                                        int offset = 0;
                                        const int BufferSize = 4096;
                                        int lastPercentage = 0;
                                        while (offset < imageBytes.Length)
                                        {
                                            int bytesToWrite = Math.Min(BufferSize, imageBytes.Length - offset);
                                            reqStream.Write(imageBytes, offset, bytesToWrite);
                                            offset += bytesToWrite;

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
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                }
                                finally
                                {
                                    resetEvent.Set();
                                }
                            }), null);

                    resetEvent.WaitOne();

                    if (asyncException != null)
                        throw asyncException;

                    resetEvent.Reset();

                    req.BeginGetResponse(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    lock (this.asyncCallbackLock)
                                    {
                                        using (var res = req.EndGetResponse(ar) as HttpWebResponse)
                                        {
                                            httpStatus = res.Headers["Status"];
                                            response = GetTwitterResponse(res);

                                            if (AsyncCallback != null)
                                            {
                                                List<T> responseObj = reqProc.ProcessResults(response);
                                                var asyncResp = new TwitterAsyncResponse<T>();
                                                asyncResp.State = responseObj.FirstOrDefault();
                                                (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
                                            }

                                            // almost done
                                            OnUploadProgressChanged(99);
                                        } 
                                    }
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                }
                                finally
                                {
#if !WINDOWS_PHONE && !NETFX_CORE
                                   resetEvent.Set();
#endif
                                }
                            }), null);

#if !WINDOWS_PHONE && !NETFX_CORE
                    resetEvent.WaitOne();
#endif

                    if (asyncException != null)
                        throw asyncException;
                }
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            // make sure the caller knows it's done
            OnUploadProgressChanged(100);

            CheckResultsForTwitterError(response, httpStatus);

            return response;
        }

        /// <summary>
        /// performs HTTP POST media byte array upload to Twitter
        /// </summary>
        /// <param name="url">url to upload to</param>
        /// <param name="postData">request parameters</param>
        /// <param name="mediaItems">list of Media each media item to upload</param>
        /// <param name="reqProc">request processor for handling results</param>
        /// <returns>XML results From Twitter</returns>
        public string PostMedia<T>(string url, IDictionary<string, string> postData, List<Media> mediaItems, IRequestProcessor<T> reqProc)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

            var formDataSb = new StringBuilder();

            if (postData != null && postData.Count > 0)
            {
                foreach (var param in postData)
                {
                    if (param.Value != null)
                    {
                        byte[] paramBytes = Encoding.UTF8.GetBytes(param.Value);
                        string encodedParamVal = encoding.GetString(paramBytes, 0, paramBytes.Length);

                        formDataSb.AppendFormat(
                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                            contentBoundaryBase, param.Key, encodedParamVal);
                    }
                }
            }

            foreach (var media in mediaItems)
            {
                formDataSb.Append(beginContentBoundary);
                formDataSb.Append(
                    string.Format(
                        "Content-Disposition: form-data; name=\"media[]\"; " +
                        "filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n",
                        media.FileName, media.ContentType));
                formDataSb.Append(encoding.GetString(media.Data, 0, media.Data.Length));
            }

            formDataSb.Append(endContentBoundary);

            byte[] imageBytes = encoding.GetBytes(formDataSb.ToString());

            string response = string.Empty;
            string httpStatus = string.Empty;

            try
            {
                LastUrl = url;

                //Log
                WriteLog(LastUrl, "PostMedia");

                var dontIncludePostParametersInOAuthSignature = new Dictionary<string, string>();
                var req = AuthorizedClient.PostRequest(new Request(url), dontIncludePostParametersInOAuthSignature);
                req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
#if !WINDOWS_PHONE && !NETFX_CORE
                req.AllowWriteStreamBuffering = true;
                req.ContentLength = imageBytes.Length; 
#endif

                Exception asyncException = null;
                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
                {
                    req.BeginGetRequestStream(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    using (var reqStream = req.EndGetRequestStream(ar))
                                    {
                                        int offset = 0;
                                        const int BufferSize = 4096;
                                        int lastPercentage = 0;
                                        while (offset < imageBytes.Length)
                                        {
                                            int bytesToWrite = Math.Min(BufferSize, imageBytes.Length - offset);
                                            reqStream.Write(imageBytes, offset, bytesToWrite);
                                            offset += bytesToWrite;

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
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                }
                                finally
                                {
                                    resetEvent.Set();
                                }
                            }), null);

                    resetEvent.WaitOne();

                    if (asyncException != null)
                        throw asyncException;

                    resetEvent.Reset();

                    req.BeginGetResponse(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    lock (this.asyncCallbackLock)
                                    {
                                        using (var res = req.EndGetResponse(ar) as HttpWebResponse)
                                        {
                                            httpStatus = res.Headers["Status"];
                                            response = GetTwitterResponse(res);

                                            if (AsyncCallback != null)
                                            {
                                                T responseObj = (reqProc as IRequestProcessorWithAction<T>).ProcessActionResult(response, StatusAction.SingleStatus);
                                                var asyncResp = new TwitterAsyncResponse<T>();
                                                asyncResp.State = responseObj;
                                                (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
                                            }

                                            // almost done
                                            OnUploadProgressChanged(99);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                }
                                finally
                                {
#if !WINDOWS_PHONE && !NETFX_CORE
                                    resetEvent.Set();
#endif
                                }
                            }), null);

#if !WINDOWS_PHONE && !NETFX_CORE
                    resetEvent.WaitOne();
#endif
                    if (asyncException != null)
                        throw asyncException;
                }
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            // make sure the caller knows it's done
            OnUploadProgressChanged(100);

            CheckResultsForTwitterError(response, httpStatus);

            return response;
        }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="postData">Name/value pairs of parameters</param>
        /// <param name="reqProc">Processes results of async requests</param>
        /// <returns>XML response from Twitter</returns>
        public string ExecuteTwitter<T>(string url, IDictionary<string, string> postData, Func<string, T> getResult)
        {
            string httpStatus = string.Empty;
            string response = string.Empty;

            try
            {
                LastUrl = url;
                WriteLog(LastUrl, "ExecuteTwitter");
                var request = new Request(url);

#if !SILVERLIGHT
                if (AsyncCallback != null)
                {
#endif
                    HttpWebRequest req = AuthorizedClient.PostAsync(request, postData);

#if !SILVERLIGHT
                    IAsyncResult arResp = 
#endif
                        req.BeginGetResponse(
                          new AsyncCallback(
                            ar =>
                            {
                                lock (asyncCallbackLock)
                                {
                                    var asyncResp = new TwitterAsyncResponse<T>();
                                    try
                                    {
                                        var resp = req.EndGetResponse(ar) as HttpWebResponse;
                                        response = GetTwitterResponse(resp);
                                        CheckResultsForTwitterError(response, httpStatus);
                                    }
                                    catch (Exception ex)
                                    {
                                        asyncResp.Status = TwitterErrorStatus.RequestProcessingException;
                                        asyncResp.Message = "Processing failed. See Error property for more details.";
                                        asyncResp.Error = ex;
                                    }
                                    finally
                                    {
                                        asyncResp.State = getResult(response);
                                        (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
                                        AsyncCallback = null;
                                    }
                                }
                            }),
                            null);

#if !SILVERLIGHT && !NETFX_CORE
                    if (AsyncCallback == null)
                        ThreadPool.RegisterWaitForSingleObject(arResp.AsyncWaitHandle,
                            (state, timedOut) =>
                            {
                                lock (asyncCallbackLock)
                                {
                                    if (timedOut)
                                    {
                                        var reqState = state as HttpWebRequest;
                                        if (reqState != null)
                                        {
                                            reqState.Abort();
                                            var asyncResp = new TwitterAsyncResponse<T>();
                                            asyncResp.Error = new TwitterQueryException("Async query timed out.", asyncResp.Error);
                                            (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
                                        }
                                    } 
                                }
                            },
                            null,
                            Timeout,
                            true);
#endif
#if !SILVERLIGHT
                }
                else
                {
                    var req = AuthorizedClient.PostRequest(request, postData);
                    using (var resp = Utilities.AsyncGetResponse(req))
                    {
                        httpStatus = resp.Headers["Status"];
                        response = GetTwitterResponse(resp);
                        CheckResultsForTwitterError(response, httpStatus);
                    }
                }
#endif
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }

            return response;
        }

        void WriteLog(string content, string currentMethod)
        {
            if (Log != null)
            {
                Log.WriteLine("--Log Starts Here--");
                Log.WriteLine("Query:" + content);
                Log.WriteLine("Method:" + currentMethod);
                Log.WriteLine("--Log Ends Here--");
                Log.Flush();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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

                if (Log != null)
                {
                    Log.Dispose();
                }
            }
        }
    }
}
