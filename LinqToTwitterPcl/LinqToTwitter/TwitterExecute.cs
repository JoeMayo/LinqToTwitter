using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter.Net;
using LinqToTwitter.Security;

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
        const string LinqToTwitterVersion = "LINQ to Twitter v3.0";

        /// <summary>
        /// Default for ReadWriteTimeout
        /// </summary>
        public const int DefaultReadWriteTimeout = 300000;

        /// <summary>
        /// Gets or sets the object that can send authorized requests to Twitter.
        /// </summary>
        public IAuthorizer Authorizer { get; set; }

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

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                return Authorizer.UserAgent;
            }
            set
            {
                Authorizer.UserAgent =
                    string.IsNullOrEmpty(value) ?
                        Authorizer.UserAgent :
                        value + ";" + Authorizer.UserAgent;
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
        public Func<StreamContent, Task> StreamingCallbackAsync { get; set; }

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
        public TwitterExecute(IAuthorizer authorizer)
        {
            if (authorizer == null)
            {
                throw new ArgumentNullException("authorizedClient");
            }

            Authorizer = authorizer;
            Authorizer.UserAgent = Authorizer.UserAgent ?? LinqToTwitterVersion;
        }



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
        /// Used in queries to read information from Twitter API endpoints.
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Respose from Twitter</returns>
        public async Task<string> QueryTwitterAsync<T>(Request request, IRequestProcessor<T> reqProc)
        {
            WriteLog(LastUrl, "QueryTwitterAsync");

            var req = new HttpRequestMessage(HttpMethod.Get, request.FullUrl);

            var parms = request.RequestParameters
                               .ToDictionary(
                                    key => key.Name,
                                    val => val.Value);
            SetAuthorizationHeader(HttpMethod.Get, request.FullUrl, parms, req);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;

            using (var client = new HttpClient())
            {
                var msg = await client.SendAsync(req);

                await TwitterErrorHandler.ThrowIfErrorAsync(msg);

                return await msg.Content.ReadAsStringAsync();
            }
        }
  
        internal void SetAuthorizationHeader(HttpMethod method, string url, IDictionary<string, string> parms, HttpRequestMessage req)
        {
            var authStringParms = parms.ToDictionary(parm => parm.Key, elm => elm.Value);
            authStringParms.Add("oauth_consumer_key", Authorizer.CredentialStore.ConsumerKey);
            authStringParms.Add("oauth_token", Authorizer.CredentialStore.OAuthToken);

            string authorizationString = Authorizer.GetAuthorizationString(method, url, authStringParms);

            req.Headers.Add("Authorization", authorizationString);
        }

        /// <summary>
        /// Performs a query on the Twitter Stream.
        /// </summary>
        /// <param name="request">Request with url endpoint and all query parameters.</param>
        /// <returns>
        /// Caller expects an JSON formatted string response, but
        /// real response(s) with streams is fed to the callback.
        /// </returns>
        public async Task<string> QueryTwitterStreamAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                var parameters =
                    (from parm in request.RequestParameters
                     select new KeyValuePair<string, string>(parm.Name, parm.Value))
                    .ToList();
                var formUrlEncodedContent = new FormUrlEncodedContent(parameters);

                formUrlEncodedContent.Headers.ContentType =
                    new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Endpoint);
                httpRequest.Content = formUrlEncodedContent;

                var parms = request.RequestParameters
                                  .ToDictionary(
                                       key => key.Name,
                                       val => val.Value);
                SetAuthorizationHeader(HttpMethod.Post, request.FullUrl, parms, httpRequest);
                httpRequest.Headers.Add("User-Agent", UserAgent);
                httpRequest.Headers.ExpectContinue = false;

                var response = httpClient.SendAsync(
                    httpRequest, HttpCompletionOption.ResponseHeadersRead).Result;
                var stream = response.Content.ReadAsStreamAsync().Result;

                using (var reader = new StreamReader(stream))
                {

                    while (!reader.EndOfStream)
                    {
                        var content = reader.ReadLine();

                        var strmContent = new StreamContent(this, content);
                        await StreamingCallbackAsync(strmContent);
                    }
                }
            }
            
            return "<streaming></streaming>";
        }

        /// <summary>
        /// Performs HTTP POST media byte array upload to Twitter.
        /// </summary>
        /// <param name="url">Url to upload to.</param>
        /// <param name="postData">Request parameters.</param>
        /// <param name="data">Image to upload.</param>
        /// <param name="name">Image parameter name.</param>
        /// <param name="fileName">Image file name.</param>
        /// <param name="contentType">Type of image: must be one of jpg, gif, or png.</param>
        /// <param name="reqProc">Request processor for handling results.</param>
        /// <returns>JSON response From Twitter.</returns>
        public async Task<string> PostMediaAsync(string url, IDictionary<string, string> postData, byte[] data, string name, string fileName, string contentType)
        {
            var multiPartContent = new MultipartFormDataContent();
            var byteArrayContent = new ByteArrayContent(data);
            byteArrayContent.Headers.Add("Content-Type", contentType);
            multiPartContent.Add(byteArrayContent, name, fileName);

            var cleanPostData = new Dictionary<string, string>();

            foreach (var pair in postData)
            {
                if (pair.Value != null)
                {
                    cleanPostData.Add(pair.Key, pair.Value);
                    multiPartContent.Add(new StringContent(pair.Value), pair.Key);
                }
            }

            var handler = new PostMessageHandler(this, new Dictionary<string, string>(), url);
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage msg = await client.PostAsync(url, multiPartContent);

                await TwitterErrorHandler.ThrowIfErrorAsync(msg);

                return await msg.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// performs HTTP POST to Twitter
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="postData">parameters to post</param>
        /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
        /// <returns>Json Response from Twitter - empty string if async</returns>
        public async Task<string> PostToTwitterAsync<T>(string url, IDictionary<string, string> postData)
        {
            WriteLog(LastUrl, "PostToTwitterAsync");

            var cleanPostData = new Dictionary<string, string>();

            foreach (var pair in postData)
            {
                if (pair.Value != null)
                    cleanPostData.Add(pair.Key, pair.Value);
            }

            var content = new FormUrlEncodedContent(cleanPostData);
            var handler = new PostMessageHandler(this, cleanPostData, url);
            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage msg = await client.PostAsync(url, content);

                await TwitterErrorHandler.ThrowIfErrorAsync(msg);

                return await msg.Content.ReadAsStringAsync();
            }
        }

        class PostMessageHandler : DelegatingHandler
        {
            TwitterExecute exe;
            IDictionary<string, string> postData;
            string url;

            public PostMessageHandler(TwitterExecute exe, IDictionary<string, string> postData, string url)
                : base(new HttpClientHandler())
            {
                this.exe = exe;
                this.postData = postData;
                this.url = url;
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                exe.SetAuthorizationHeader(HttpMethod.Post, url, postData, request);
                request.Headers.Add("User-Agent", exe.UserAgent);
                request.Headers.ExpectContinue = false;
                //request.Headers.Add("Accept-Encoding", "gzip");
                
                return await base.SendAsync(request, cancellationToken);
            }
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
                var disposableClient = Authorizer as IDisposable;
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
