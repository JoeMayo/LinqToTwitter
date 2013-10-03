using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//using LinqToTwitter.Common;
//using LitJson;

#if NETFX_CORE
using System.Threading.Tasks;
#endif

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE && !L2T_PCL
#endif

//#if !SILVERLIGHT && !L2T_PCL
//using System.IO.Compression;
//#else
//using Ionic.Zlib;
//#endif

namespace LinqToTwitter
{
    /// <summary>
    /// Logic that performs actual communication with Twitter
    /// </summary>
    internal partial class TwitterExecute : ITwitterExecute, IDisposable
    {
        /// <summary>
        /// Version used in UserAgent
        /// </summary>
        const string LinqToTwitterVersion = "LINQ to Twitter v2.1";

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

//        /// <summary>
//        /// Gets or sets the object that can send authorized requests to Twitter.
//        /// </summary>
//        public ITwitterAuthorizer AuthorizedClient { get; set; }

//        /// <summary>
//        /// Timeout (milliseconds) for writing to request 
//        /// stream or reading from response stream
//        /// </summary>
//        public int ReadWriteTimeout
//        {
//            get { return (int)AuthorizedClient.ReadWriteTimeout.TotalMilliseconds; }
//            set { AuthorizedClient.ReadWriteTimeout = TimeSpan.FromMilliseconds(value); }
//        }

//        /// <summary>
//        /// Default for Timeout
//        /// </summary>
//        public const int DefaultTimeout = 100000;

//        /// <summary>
//        /// Timeout (milliseconds) to wait for a server response
//        /// </summary>
//        public int Timeout
//        {
//            get { return (int)AuthorizedClient.Timeout.TotalMilliseconds; }
//            set { AuthorizedClient.Timeout = TimeSpan.FromMilliseconds(value); }
//        }

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
        public IDictionary<string, string> ResponseHeaders { get; set; }

//        /// <summary>
//        /// Gets and sets HTTP UserAgent header
//        /// </summary>
//        public string UserAgent
//        {
//            get
//            {
//                return AuthorizedClient.UserAgent;
//            }
//            set
//            {
//                AuthorizedClient.UserAgent =
//                    string.IsNullOrEmpty(value) ?
//                        AuthorizedClient.UserAgent :
//                        value + ";" + AuthorizedClient.UserAgent;
//            }
//        }

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

//        /// <summary>
//        /// supports testing
//        /// </summary>
//        public TwitterExecute(ITwitterAuthorizer authorizedClient)
//        {
//            if (authorizedClient == null)
//            {
//                throw new ArgumentNullException("authorizedClient");
//            }

//            AuthorizedClient = authorizedClient;
//            AuthorizedClient.UserAgent = LinqToTwitterVersion;
//        }

//        /// <summary>
//        /// Common code to construct a TwitterQueryException instance
//        /// </summary>
//        /// <param name="responseStr">Response from Twitter</param>
//        /// <param name="wex">WebException assigned as InnerException if available</param>
//        /// <returns>An Instance of TwitterQueryException</returns>
//        TwitterQueryException ConstructTwitterQueryException(string responseStr, WebException wex)
//        {
//            JsonData responseJson = JsonMapper.ToObject(responseStr);

//            TwitterQueryException twitterQueryEx = null;

//            var errors = responseJson.GetValue<JsonData>("errors");
//            if (errors != null && errors.Count > 0)
//            {
//                var error = errors[0];
//                twitterQueryEx = new TwitterQueryException(error.GetValue<string>("message"), wex)
//                {
//                    HttpError = wex == null ? string.Empty : wex.Status.ToString(),
//                    ErrorCode = error.GetValue<int>("code")
//                };
//            }
//            else
//            {
//                twitterQueryEx = new TwitterQueryException("Error while querying Twitter.", wex);
//            }

//            return twitterQueryEx;
//        }

//        /// <summary>
//        /// Throws exception if error returned from Twitter
//        /// </summary>
//        /// <param name="responseStr">XML or JSON string response from Twitter</param>
//        /// <param name="status">HTTP Error number</param>
//        internal void CheckResultsForTwitterError(string responseStr, string status)
//        {
//            if (responseStr.StartsWith("{", StringComparison.Ordinal))
//            {
//                TwitterQueryException twitterQueryEx = ConstructTwitterQueryException(responseStr, null);

//                if (twitterQueryEx.ErrorCode != 0)
//                    throw twitterQueryEx;
//            }
//        }

//        /// <summary>
//        /// generates a new TwitterQueryException from a WebException
//        /// </summary>
//        /// <param name="wex">Web Exception to Translate</param>
//        /// <returns>new TwitterQueryException instance</returns>
//        internal TwitterQueryException CreateTwitterQueryException(WebException wex)
//        {
//            const string DefaultResponse = @"{""errors"":[{""message"":""No message from Twitter"",""code"":0}]}";
//            string responseStr = DefaultResponse;

//            try
//            {
//                if (wex != null && wex.Response != null)
//                {
//                    responseStr = GetTwitterResponse(wex.Response);
//                }
//            }
//            catch (Exception)
//            {
//                responseStr = DefaultResponse;
//            }

//            if (!responseStr.StartsWith("{", StringComparison.Ordinal))
//                responseStr = DefaultResponse;

//            TwitterQueryException twitterQueryEx = ConstructTwitterQueryException(responseStr, wex);

//            return twitterQueryEx;
//        }

//        string ReadStreamBytes(Stream stream)
//        {
//            const int ByteCount = 4096;
//            var sb = new StringBuilder();

//            using (var reader = new StreamReader(stream))
//            {
//                while (reader.Peek() >= 0)
//                {
//                    var buffer = new char[ByteCount];
//                    reader.ReadBlock(buffer, 0, ByteCount);
//                    sb.Append(buffer);
//                }
//            }

//            return sb.ToString().Trim('\0');
//        }

//        /// <summary>
//        /// gets WebResponse contents from Twitter
//        /// </summary>
//        /// <param name="resp">WebResponse to extract string from</param>
//        /// <returns>XML string response from Twitter</returns>
//        string GetTwitterResponse(WebResponse resp)
//        {
//            string responseBody;

//            using (var respStream = resp.GetResponseStream())
//            {
//                string contentEncoding = string.Empty;

//#if !SILVERLIGHT
//                contentEncoding = resp.Headers["Content-Encoding"] ?? "";
//#endif

//                if (contentEncoding.ToLower().Contains("gzip"))
//                {
//                    using (var gzip = new GZipStream(respStream, CompressionMode.Decompress))
//                    {
//                        responseBody = ReadStreamBytes(gzip);
//                    }
//                }
//                else if (resp.ContentType.StartsWith("image"))
//                {
//                    responseBody = "{ \"imageUrl\": \"" + resp.ResponseUri.ToString() + "\" }";
//                }
//                else
//                {
//                    responseBody = ReadStreamBytes(respStream);
//                }
//            }

//            var responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

//#if !SILVERLIGHT
//            foreach (string key in resp.Headers.AllKeys)
//            {
//                responseHeaders.Add(key, resp.Headers[key].ToString());
//            }
//#endif

//            ResponseHeaders = responseHeaders;

//            return responseBody;
//        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Respose from Twitter</returns>
        public async Task<string> QueryTwitter<T>(Request request, IRequestProcessor<T> reqProc)
        {
            await Task.Delay(1);
//#if SILVERLIGHT && !NETFX_CORE
//            if (AsyncCallback == null)
//                throw new InvalidOperationException("Silverlight and Windows Phone applications require async queries.");
//#endif
//            //Log
//            LastUrl = request.FullUrl;
//            WriteLog(LastUrl, "QueryTwitter");

            string response = string.Empty;
//            string httpStatus = string.Empty;
//            Exception thrownException = null;

//            try
//            {
//                var req = AuthorizedClient.Get(request);
//                //var req = GetHttpRequest(request);
//#if !SILVERLIGHT
//                bool initialStateSignaled = AsyncCallback != null;

//                using (var resetEvent = new ManualResetEvent(initialStateSignaled))
//                {
//#endif
//                    req.BeginGetResponse(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                lock (asyncCallbackLock)
//                                {
//                                    var asyncResp = new TwitterAsyncResponse<IEnumerable<T>>();
//                                    try
//                                    {
//                                        var res = req.EndGetResponse(ar) as HttpWebResponse;
//                                        httpStatus = (int)res.StatusCode + " " + res.StatusDescription;
//                                        response = GetTwitterResponse(res);

//                                        if (AsyncCallback != null)
//                                            asyncResp.State = reqProc.ProcessResults(response);
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        if (AsyncCallback == null)
//                                            thrownException = ex;

//                                        asyncResp.Status = TwitterErrorStatus.RequestProcessingException;
//                                        asyncResp.Message = "Processing failed. See Error property for more details.";
//                                        asyncResp.Exception = ex;
//                                    }
//                                    finally
//                                    {
//                                        if (AsyncCallback != null)
//                                        {
//                                            if (AsyncCallback is Action<IEnumerable<T>>)
//                                                (AsyncCallback as Action<IEnumerable<T>>)(asyncResp.State);
//                                            else
//                                                (AsyncCallback as Action<TwitterAsyncResponse<IEnumerable<T>>>)(asyncResp);

//                                            AsyncCallback = null;
//                                        }
//#if !SILVERLIGHT
//                                        else
//                                            resetEvent.Set();
//#endif
//                                    }
//                                }
//                            }), null);
//#if !SILVERLIGHT
//                    if (AsyncCallback == null)
//                    {
//                        resetEvent.WaitOne();

//                        if (thrownException != null)
//                            throw thrownException;
//                    }
//                }
//#endif
//            }
//            catch (WebException wex)
//            {
//                var twitterQueryEx = CreateTwitterQueryException(wex);
//                throw twitterQueryEx;
//            }

//            CheckResultsForTwitterError(response, httpStatus);

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
//#if NETFX_CORE
//            Task.Run(() => ExecuteTwitterStream(request));
//#else
//            ThreadPool.QueueUserWorkItem(ExecuteTwitterStream, request);
//#endif
            return "<streaming></streaming>";
        }

//        /// <summary>
//        /// Processes stream results and performs error handling
//        /// </summary>
//        /// <param name="state">The request</param>
//        void ExecuteTwitterStream(object state)
//        {
//            var request = state as Request;
//            var streamUrl = request.Endpoint;

//            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
//            {
//                try
//                {
//                    HttpWebRequest req = null;

//                    if (streamUrl.Contains("user.json") || streamUrl.Contains("site.json"))
//                    {
//                        req = GetUserStreamRequest(request);
//                    }
//                    else
//                    {
//                        req = GetHttpRequest(request);
//                    }

//                    req.BeginGetResponse(
//                        new AsyncCallback(ar =>
//                        {
//                            HttpWebResponse resp = null;

//                            try
//                            {
//                                resp = req.EndGetResponse(ar) as HttpWebResponse;

//                                using (var stream = resp.GetResponseStream())
//                                using (MemoryStream memory = new MemoryStream())
//                                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Decompress))
//                                {
//                                    byte[] compressedBuffer = new byte[8192];
//                                    byte[] uncompressedBuffer = new byte[8192];
//                                    List<byte> output = new List<byte>();

//                                    try
//                                    {
//                                        lock (streamingCallbackLock)
//                                        {
//                                            while (stream.CanRead && !CloseStream)
//                                            {
//                                                int readCount = stream.Read(compressedBuffer, 0, compressedBuffer.Length);

//                                                // When Twitter breaks the connection, we need to exit the
//                                                // entire loop and start over. Otherwise, the reads
//                                                // keep returning blank lines that are incorrectly interpreted
//                                                // as keep-alive messages in a tight loop.
//                                                if (readCount == 0)
//                                                {
//                                                    if (!CloseStream)
//                                                    {
//                                                        CloseStream = true;
//                                                        throw new WebException("Twitter closed the stream.", WebExceptionStatus.ConnectFailure);
//                                                    }

//                                                    return;
//                                                }

//                                                memory.Write(compressedBuffer.Take(readCount).ToArray(), 0, readCount);
//                                                memory.Position = 0;

//                                                int uncompressedLength = 0;

//                                                if (resp.Headers["content-encoding"] != null &&
//                                                    resp.Headers["content-encoding"].Contains("gzip"))
//                                                {
//                                                    uncompressedLength = gzip.Read(uncompressedBuffer, 0, uncompressedBuffer.Length);
//                                                }
//                                                else
//                                                {
//                                                    compressedBuffer.CopyTo(uncompressedBuffer, 0);
//                                                    uncompressedLength = compressedBuffer.Length;
//                                                }

//                                                output.AddRange(uncompressedBuffer.Take(uncompressedLength));

//                                                if (!output.Contains(0x0D)) continue;

//                                                byte[] bytesToDecode = output.Take(output.LastIndexOf(0x0D) + 2).ToArray();
//                                                string outputString = Encoding.UTF8.GetString(bytesToDecode, 0, bytesToDecode.Length);
//                                                output.RemoveRange(0, bytesToDecode.Length);

//                                                string[] lines = outputString.Split(new[] { "\r\n" }, new StringSplitOptions());
//                                                for (int i = 0; i < (lines.Length - 1); i++)
//                                                {
//                                                    DoAsyncCallback(lines[i]);
//                                                }

//                                                compressedBuffer = new byte[8192];
//                                                uncompressedBuffer = new byte[8192];
//                                                output = new List<byte>();
//                                                memory.SetLength(0);
//                                            }
//                                        }
//                                    }
//                                    finally
//                                    {
//                                        if (req != null) req.Abort();
//                                    }
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                WriteLog(ex.ToString(), "ExecuteTwitterStream");
//                                DoAsyncCallback(ex);
//                            }
//                            finally
//                            {
//                                if (req != null) req.Abort();
//                                if (resetEvent != null) resetEvent.Set();
//                            }
//                        }), null);

//                    resetEvent.WaitOne();
//                    resetEvent.Reset();
//                }
//                finally
//                {
//                    // user might want to try to reconnect with
//                    // the same instance of TwitterContext
//                    CloseStream = false;
//                }
//            }
//        }

//        /// <summary>
//        /// Handles request initialization for sample, filter, and other basic streams
//        /// </summary>
//        /// <param name="request">Stream endpoint and parameters</param>
//        /// <returns>Initialized Request</returns>
//        HttpWebRequest GetHttpRequest(Request request)
//        {
//            HttpWebRequest req = null;
//            byte[] bytes = new byte[0];
//            LastUrl = request.FullUrl;
//            bool shouldPostQuery =
//                LastUrl.Contains("filter.json") ||
//                (LastUrl.Contains("/site/c/") && LastUrl.Contains("friends/ids.json"));

//            if (shouldPostQuery)
//            {
//                var postData =
//                    (from data in request.RequestParameters
//                     select new
//                     {
//                         data.Name,
//                         data.Value
//                     })
//                    .ToDictionary(key => key.Name, val => val.Value);

//                req = this.AuthorizedClient.PostRequest(request, null);
//                int qIndex = LastUrl.IndexOf('?');

//                string urlParams = LastUrl.Substring(qIndex + 1);
//                bytes = Encoding.UTF8.GetBytes(urlParams);

//                req.Method = "POST";
//                req.ContentType = "application/x-www-form-urlencoded";

//#if !WINDOWS_PHONE && !NETFX_CORE
//                req.ContentLength = bytes.Length;
//#endif

//#if !SILVERLIGHT && !NETFX_CORE
//                if (Timeout > 0)
//                    req.Timeout = Timeout;

//                if (ReadWriteTimeout > 0)
//                    req.ReadWriteTimeout = ReadWriteTimeout;

//                req.AutomaticDecompression = DecompressionMethods.None;
//#endif
//#if WINDOWS_PHONE
//                req.AllowReadStreamBuffering = false;
//#endif
//                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
//                {
//                    Exception asyncException = null;

//                    req.BeginGetRequestStream(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    using (var requestStream = req.EndGetRequestStream(ar))
//                                    {
//                                        requestStream.Write(bytes, 0, bytes.Length);
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                    WriteLog(ex.ToString(), "GetBasicStreamRequest");
//                                    throw;
//                                }

//                                resetEvent.Set();

//                            }), null);

//                    resetEvent.WaitOne();

//                    if (asyncException != null)
//                        throw asyncException;
//                }
//            }
//            else
//            {
//                req = this.AuthorizedClient.Get(request) as HttpWebRequest;
//            }

//#if !SILVERLIGHT && !NETFX_CORE
//            req.UserAgent = UserAgent;
//#endif

//            return req;
//        }

//        /// <summary>
//        /// Handles request initialization for user and site streams
//        /// </summary>
//        /// <param name="request">Stream endpoint and parameters</param>
//        /// <returns>Initialized Request</returns>
//        HttpWebRequest GetUserStreamRequest(Request request)
//        {
//            this.LastUrl = request.FullUrl;
//            var req = this.AuthorizedClient.Get(request) as HttpWebRequest;
//#if !SILVERLIGHT && !NETFX_CORE
//            req.UserAgent = UserAgent;
//            req.AutomaticDecompression = DecompressionMethods.None;
//#endif
//#if WINDOWS_PHONE
//            req.AllowReadStreamBuffering = false;
//#endif

//            return req;
//        }

//        void DoAsyncCallback(object state)
//        {
//#if NETFX_CORE
//            Task.Run(() => InvokeStreamCallback(state));
//#else
//            ThreadPool.QueueUserWorkItem(InvokeStreamCallback, state);
//#endif
//        }

//        /// <summary>
//        /// Executes callback handler
//        /// </summary>
//        /// <remarks>
//        /// If the user's callback code fails to handle an exception
//        /// this code will log and re-throw.  The user should consider
//        /// ensuring the code they write doesn't do anything
//        /// that will get them rate-limited or black-listed on Twitter.
//        /// </remarks>
//        /// <param name="content">Content from Twitter</param>
//        void InvokeStreamCallback(object content)
//        {
//            try
//            {
//                StreamContent strmContent = null;

//                if (content is string)
//                {
//                    strmContent = new StreamContent(this, content as string);
//                }
//                else
//                {
//                    const string Message =
//                        "An error has occurred during processing. " +
//                        "This is sometimes an unknown error. " +
//                        "It can also happen when Twitter closes the stream. " +
//                        "Whatever the cause, your stream has been closed. " +
//                        "You can find more information about this exception in the 'Error' property.";

//                    strmContent = new StreamContent(this, Message);
//                    strmContent.Status = TwitterErrorStatus.RequestProcessingException;
//                    strmContent.Error = content as Exception;
//                }

//                StreamingCallback(strmContent);
//            }
//            catch (Exception ex)
//            {
//                WriteLog("Unhandled exception in your StreamingCallback code.  " + ex, "InvokeCallback");
//                throw;
//            }
//        }

//#if !NETFX_CORE
//        /// <summary>
//        /// performs HTTP POST file upload to Twitter
//        /// </summary>
//        /// <param name="url">url to upload to</param>
//        /// <param name="postData">query string parameters</param>
//        /// <param name="filePath">full path of file to upload</param>
//        /// <param name="reqProc">Processes results of async requests</param>
//        /// <returns>XML Respose from Twitter</returns>
//        public string PostTwitterFile<T>(string url, IDictionary<string, string> postData, string filePath, IRequestProcessor<T> reqProc)
//        {
//            var fileName = Path.GetFileName(filePath);

//            string imageType;

//            switch (Path.GetExtension(fileName).ToLower())
//            {
//                case ".jpg":
//                case ".jpeg":
//                    imageType = "jpg";
//                    break;
//                case ".gif":
//                    imageType = "gif";
//                    break;
//                case ".png":
//                    imageType = "png";
//                    break;
//                default:
//                    throw new ArgumentException(
//                        "Can't recognize the extension of the file you're uploading. Please choose either a *.gif, *.jpg, *.jpeg, or *.png file.", filePath);
//            }

//            byte[] fileBytes = Utilities.GetFileBytes(filePath);

//            return PostTwitterImage(url, postData, fileBytes, fileName, imageType, reqProc);
//        }
//#endif

//        /// <summary>
//        /// performs HTTP POST image byte array upload to Twitter
//        /// </summary>
//        /// <param name="url">url to upload to</param>
//        /// <param name="postData">parameters to pass</param>
//        /// <param name="image">byte array containing image to upload</param>
//        /// <param name="fileName">name to pass to Twitter for the file</param>
//        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
//        /// <param name="reqProc">Processes results of async requests</param>
//        /// <returns>XML Response from Twitter</returns>
//        public string PostTwitterImage<T>(string url, IDictionary<string, string> postData, byte[] image, string fileName, string imageType, IRequestProcessor<T> reqProc)
//        {
//#if SILVERLIGHT && !NETFX_CORE
//            if (AsyncCallback == null)
//                throw new InvalidOperationException("Silverlight and Windows Phone applications require async commands.");
//#endif
//            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
//            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
//            var contentDisposition = string.Format("Content-Disposition:form-data; name=\"image\"; filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", fileName, imageType);
//            //var contentDisposition = string.Format("Content-Disposition:form-data); name=\"image\"); filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", fileName, imageType);
//            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

//            var formDataSb = new StringBuilder();

//            if (postData != null && postData.Count > 0)
//            {
//                foreach (var param in postData)
//                {
//                    if (param.Value == "IMAGE_DATA")
//                    {
//                        contentDisposition = contentDisposition.Replace("name=\"image\"", "name=\"" + param.Key + "\"");
//                    }
//                    else
//                    {
//                        formDataSb.AppendFormat("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", contentBoundaryBase, param.Key, param.Value);
//                    }
//                }
//            }

//            var encoding = Encoding.GetEncoding("iso-8859-1");
//            string imageByteString = encoding.GetString(image, 0, image.Length);

//            byte[] imageBytes =
//                encoding.GetBytes(
//                    formDataSb.ToString() +
//                    beginContentBoundary +
//                    contentDisposition +
//                    imageByteString +
//                    endContentBoundary);

//            string response = string.Empty;
//            string httpStatus = string.Empty;

//            try
//            {
//                LastUrl = url;
//                //Log
//                WriteLog(LastUrl, "PostTwitterImage");

//                var req = AuthorizedClient.PostRequest(new Request(url), new Dictionary<string, string>());
//                req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;

//#if !WINDOWS_PHONE && !NETFX_CORE
//                req.AllowWriteStreamBuffering = true;
//                req.ContentLength = imageBytes.Length;
//#endif

//                Exception asyncException = null;
//                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
//                {

//                    req.BeginGetRequestStream(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    using (var reqStream = req.EndGetRequestStream(ar))
//                                    {
//                                        int offset = 0;
//                                        const int BufferSize = 4096;
//                                        int lastPercentage = 0;
//                                        while (offset < imageBytes.Length)
//                                        {
//                                            int bytesToWrite = Math.Min(BufferSize, imageBytes.Length - offset);
//                                            reqStream.Write(imageBytes, offset, bytesToWrite);
//                                            offset += bytesToWrite;

//                                            int percentComplete =
//                                                (int)((double)offset / (double)imageBytes.Length * 100);

//                                            // since we still need to get the response later
//                                            // in the algorithm, interpolate the results to
//                                            // give user a more accurate picture of completion.
//                                            // i.e. we don't want to shoot up to 100% here when
//                                            // we know there is more processing to do.
//                                            lastPercentage = percentComplete >= 98 ?
//                                                100 - ((98 - lastPercentage) / 2) :
//                                                percentComplete;

//                                            OnUploadProgressChanged(lastPercentage);
//                                        }

//                                        reqStream.Flush();
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                }
//                                finally
//                                {
//                                    resetEvent.Set();
//                                }
//                            }), null);

//                    resetEvent.WaitOne();

//                    if (asyncException != null)
//                        throw asyncException;

//                    resetEvent.Reset();

//                    req.BeginGetResponse(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    lock (this.asyncCallbackLock)
//                                    {
//                                        using (var res = req.EndGetResponse(ar) as HttpWebResponse)
//                                        {
//                                            httpStatus = res.Headers["Status"];
//                                            response = GetTwitterResponse(res);

//                                            if (AsyncCallback != null)
//                                            {
//                                                List<T> responseObj = reqProc.ProcessResults(response);
//                                                var asyncResp = new TwitterAsyncResponse<T>();
//                                                asyncResp.State = responseObj.FirstOrDefault();
//                                                (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
//                                                AsyncCallback = null; // set to null because (unlikely, but possible) the next query might not be async
//                                            }

//                                            // almost done
//                                            OnUploadProgressChanged(99);
//                                        }
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                }
//                                finally
//                                {
//#if !WINDOWS_PHONE && !NETFX_CORE
//                                    resetEvent.Set();
//#endif
//                                }
//                            }), null);

//#if !WINDOWS_PHONE && !NETFX_CORE
//                    resetEvent.WaitOne();
//#endif

//                    if (asyncException != null)
//                        throw asyncException;
//                }
//            }
//            catch (WebException wex)
//            {
//                var twitterQueryEx = CreateTwitterQueryException(wex);
//                throw twitterQueryEx;
//            }

//            // make sure the caller knows it's done
//            OnUploadProgressChanged(100);

//            CheckResultsForTwitterError(response, httpStatus);

//            return response;
//        }

//        /// <summary>
//        /// performs HTTP POST media byte array upload to Twitter
//        /// </summary>
//        /// <param name="url">url to upload to</param>
//        /// <param name="postData">request parameters</param>
//        /// <param name="mediaItems">list of Media each media item to upload</param>
//        /// <param name="reqProc">request processor for handling results</param>
//        /// <returns>XML results From Twitter</returns>
//        public string PostMedia<T>(string url, IDictionary<string, string> postData, List<Media> mediaItems, IRequestProcessor<T> reqProc)
//        {
//#if SILVERLIGHT && !NETFX_CORE
//            if (AsyncCallback == null)
//                throw new InvalidOperationException("Silverlight and Windows Phone applications require async commands.");
//#endif
//            var encoding = Encoding.GetEncoding("iso-8859-1");
//            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
//            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
//            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

//            var formDataSb = new StringBuilder();

//            if (postData != null && postData.Count > 0)
//            {
//                foreach (var param in postData)
//                {
//                    if (param.Value != null)
//                    {
//                        byte[] paramBytes = Encoding.UTF8.GetBytes(param.Value);
//                        string encodedParamVal = encoding.GetString(paramBytes, 0, paramBytes.Length);

//                        formDataSb.AppendFormat(
//                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
//                            contentBoundaryBase, param.Key, encodedParamVal);
//                    }
//                }
//            }

//            foreach (var media in mediaItems)
//            {
//                formDataSb.Append(beginContentBoundary);
//                formDataSb.Append(
//                    string.Format(
//                        "Content-Disposition: form-data; name=\"media[]\"; " +
//                        "filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n",
//                        media.FileName));
//                formDataSb.Append(encoding.GetString(media.Data, 0, media.Data.Length));
//            }

//            formDataSb.Append(endContentBoundary);

//            byte[] imageBytes = encoding.GetBytes(formDataSb.ToString());

//            string response = string.Empty;
//            string httpStatus = string.Empty;

//            try
//            {
//                LastUrl = url;

//                //Log
//                WriteLog(LastUrl, "PostMedia");

//                var dontIncludePostParametersInOAuthSignature = new Dictionary<string, string>();
//                var req = AuthorizedClient.PostRequest(new Request(url), dontIncludePostParametersInOAuthSignature);
//                req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
//#if !WINDOWS_PHONE && !NETFX_CORE
//                req.AllowWriteStreamBuffering = true;
//                req.ContentLength = imageBytes.Length;
//#endif

//                Exception asyncException = null;
//                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
//                {
//                    req.BeginGetRequestStream(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    using (var reqStream = req.EndGetRequestStream(ar))
//                                    {
//                                        int offset = 0;
//                                        const int BufferSize = 4096;
//                                        int lastPercentage = 0;
//                                        while (offset < imageBytes.Length)
//                                        {
//                                            int bytesToWrite = Math.Min(BufferSize, imageBytes.Length - offset);
//                                            reqStream.Write(imageBytes, offset, bytesToWrite);
//                                            offset += bytesToWrite;

//                                            int percentComplete =
//                                                (int)((double)offset / (double)imageBytes.Length * 100);

//                                            // since we still need to get the response later
//                                            // in the algorithm, interpolate the results to
//                                            // give user a more accurate picture of completion.
//                                            // i.e. we don't want to shoot up to 100% here when
//                                            // we know there is more processing to do.
//                                            lastPercentage = percentComplete >= 98 ?
//                                                100 - ((98 - lastPercentage) / 2) :
//                                                percentComplete;

//                                            OnUploadProgressChanged(lastPercentage);
//                                        }

//                                        reqStream.Flush();
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                }
//                                finally
//                                {
//                                    resetEvent.Set();
//                                }
//                            }), null);

//                    resetEvent.WaitOne();

//                    if (asyncException != null)
//                        throw asyncException;

//                    resetEvent.Reset();

//                    req.BeginGetResponse(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    lock (this.asyncCallbackLock)
//                                    {
//                                        using (var res = req.EndGetResponse(ar) as HttpWebResponse)
//                                        {
//                                            httpStatus = res.Headers["Status"];
//                                            response = GetTwitterResponse(res);

//                                            if (AsyncCallback != null)
//                                            {
//                                                T responseObj = (reqProc as IRequestProcessorWithAction<T>).ProcessActionResult(response, StatusAction.SingleStatus);
//                                                var asyncResp = new TwitterAsyncResponse<T>();
//                                                asyncResp.State = responseObj;
//                                                (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
//                                                AsyncCallback = null; // set to null because the next query might not be async
//                                            }

//                                            // almost done
//                                            OnUploadProgressChanged(99);
//                                        }
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                }
//                                finally
//                                {
//#if !WINDOWS_PHONE && !NETFX_CORE
//                                    resetEvent.Set();
//#endif
//                                }
//                            }), null);

//#if !WINDOWS_PHONE && !NETFX_CORE
//                    resetEvent.WaitOne();
//#endif
//                    if (asyncException != null)
//                        throw asyncException;
//                }
//            }
//            catch (WebException wex)
//            {
//                var twitterQueryEx = CreateTwitterQueryException(wex);
//                throw twitterQueryEx;
//            }

//            // make sure the caller knows it's done
//            OnUploadProgressChanged(100);

//            CheckResultsForTwitterError(response, httpStatus);

//            return response;
//        }

//        /// <summary>
//        /// performs HTTP POST to Twitter
//        /// </summary>
//        /// <param name="url">URL of request</param>
//        /// <param name="postData">parameters to post</param>
//        /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
//        /// <returns>Json Response from Twitter - empty string if async</returns>
//        public string PostToTwitter<T>(string url, IDictionary<string, string> postData, Func<string, T> getResult)
//        {
//#if SILVERLIGHT && !NETFX_CORE
//            if (AsyncCallback == null)
//                throw new InvalidOperationException("Silverlight and Windows Phone applications require async commands.");
//#endif
//            var encoding = Encoding.GetEncoding("iso-8859-1");

//            var paramList = new List<string>();

//            if (postData != null && postData.Count > 0)
//            {
//                foreach (var param in postData)
//                {
//                    if (param.Value != null)
//                    {
//                        string urlEncodedValue = BuildUrlHelper.UrlEncode(param.Value);
//                        byte[] valueBytes = Encoding.UTF8.GetBytes(urlEncodedValue);
//                        string encodedParamVal = encoding.GetString(valueBytes, 0, valueBytes.Length);

//                        paramList.Add(param.Key + "=" + encodedParamVal);
//                    }
//                }
//            }

//            string postDataString = string.Join("&", paramList.ToArray());
//            byte[] paramBytes = encoding.GetBytes(postDataString);

//            string response = string.Empty;
//            string httpStatus = string.Empty;

//            try
//            {
//                LastUrl = url;

//                //Log
//                WriteLog(LastUrl, "PostToTwitter");

//                var dontIncludePostParametersInOAuthSignature = new Dictionary<string, string>();
//                var request = new Request(url);
//                foreach (var key in postData.Keys)
//                {
//                    if (postData[key] != null)
//                        request.RequestParameters.Add(new QueryParameter(key, postData[key]));
//                }
//                var req = AuthorizedClient.PostRequest(request, dontIncludePostParametersInOAuthSignature);

//#if !WINDOWS_PHONE && !NETFX_CORE
//                req.AllowWriteStreamBuffering = true;
//                req.ContentLength = paramBytes.Length;
//#endif

//                Exception asyncException = null;
//                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
//                {
//                    req.BeginGetRequestStream(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    using (var reqStream = req.EndGetRequestStream(ar))
//                                    {
//                                        int offset = 0;
//                                        const int BufferSize = 4096;
//                                        int lastPercentage = 0;
//                                        while (offset < paramBytes.Length)
//                                        {
//                                            int bytesToWrite = Math.Min(BufferSize, paramBytes.Length - offset);
//                                            reqStream.Write(paramBytes, offset, bytesToWrite);
//                                            offset += bytesToWrite;

//                                            int percentComplete =
//                                                (int)((double)offset / (double)paramBytes.Length * 100);

//                                            // since we still need to get the response later
//                                            // in the algorithm, interpolate the results to
//                                            // give user a more accurate picture of completion.
//                                            // i.e. we don't want to shoot up to 100% here when
//                                            // we know there is more processing to do.
//                                            lastPercentage = percentComplete >= 98 ?
//                                                100 - ((98 - lastPercentage) / 2) :
//                                                percentComplete;

//                                            OnUploadProgressChanged(lastPercentage);
//                                        }

//                                        reqStream.Flush();
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    asyncException = ex;
//                                }
//                                finally
//                                {
//                                    resetEvent.Set();
//                                }
//                            }), null);

//                    resetEvent.WaitOne();

//                    if (asyncException != null)
//                        throw asyncException;

//                    resetEvent.Reset();

//                    req.BeginGetResponse(
//                        new AsyncCallback(
//                            ar =>
//                            {
//                                try
//                                {
//                                    lock (this.asyncCallbackLock)
//                                    {
//                                        using (var res = req.EndGetResponse(ar) as HttpWebResponse)
//                                        {
//                                            httpStatus = res.Headers["Status"];
//                                            response = GetTwitterResponse(res);

//                                            if (AsyncCallback != null)
//                                            {
//                                                var asyncResp = new TwitterAsyncResponse<T>();
//                                                asyncResp.State = getResult(response);
//                                                (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
//                                                AsyncCallback = null; // set to null because the next query might not be async
//                                            }

//                                            // almost done
//                                            OnUploadProgressChanged(99);
//                                        }
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    if (AsyncCallback == null)
//                                    {
//                                        asyncException = ex;
//                                    }
//                                    else
//                                    {
//                                        var asyncResp = new TwitterAsyncResponse<T>();
//                                        asyncResp.Status = TwitterErrorStatus.RequestProcessingException;
//                                        asyncResp.Message = "Processing failed. See Error property for more details.";
//                                        if (ex is WebException)
//                                        {
//                                            var twitterQueryEx = CreateTwitterQueryException(ex as WebException);
//                                            asyncResp.Exception = twitterQueryEx;
//                                        }
//                                        else
//                                        {
//                                            asyncResp.Exception = ex;
//                                        }
//                                        (AsyncCallback as Action<TwitterAsyncResponse<T>>)(asyncResp);
//                                    }

//                                    WriteLog("PostToTwitter", "Error querying Twitter: " + ex.ToString());
//                                }
//                                finally
//                                {
//#if !WINDOWS_PHONE && !NETFX_CORE
//                                    resetEvent.Set();
//#endif
//                                }
//                            }), null);

//#if !WINDOWS_PHONE && !NETFX_CORE
//                    resetEvent.WaitOne();
//#endif
//                    if (asyncException != null)
//                        throw asyncException;
//                }
//            }
//            catch (WebException wex)
//            {
//                var twitterQueryEx = CreateTwitterQueryException(wex);
//                throw twitterQueryEx;
//            }

//            // make sure the caller knows it's done
//            OnUploadProgressChanged(100);

//            CheckResultsForTwitterError(response, httpStatus);

//            return response;
//        }

//        void WriteLog(string content, string currentMethod)
//        {
//            if (Log != null)
//            {
//                Log.WriteLine("--Log Starts Here--");
//                Log.WriteLine("Query:" + content);
//                Log.WriteLine("Method:" + currentMethod);
//                Log.WriteLine("--Log Ends Here--");
//                Log.Flush();
//            }
//        }

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
                //var disposableClient = this.AuthorizedClient as IDisposable;
                //if (disposableClient != null)
                //{
                //    disposableClient.Dispose();
                //}

                //if (Log != null)
                //{
                //    Log.Dispose();
                //}
            }
        }
    }
}
