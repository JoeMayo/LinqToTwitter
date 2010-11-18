using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace LinqToTwitter
{
    [Serializable]
    public abstract class OAuthAuthorizer
    {
        public OAuthAuthorizer()
        {
            OAuthRequestTokenUrl = "https://api.twitter.com/oauth/request_token";
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authorize";
            OAuthAccessTokenUrl = "https://api.twitter.com/oauth/access_token";

            OAuthTwitter = new OAuthTwitter();
            OAuthHelper = new OAuthHelper();
        }

        /// <summary>
        /// URL for OAuth Request Tokens
        /// </summary>
        public string OAuthRequestTokenUrl { get; set; }

        /// <summary>
        /// URL for OAuth authorization
        /// </summary>
        public string OAuthAuthorizeUrl { get; set; }

        /// <summary>
        /// URL for OAuth Access Tokens
        /// </summary>
        public string OAuthAccessTokenUrl { get; set; }

        /// <summary>
        /// Contains general OAuth functionality
        /// </summary>
        public IOAuthTwitter OAuthTwitter { get; set; }

        /// <summary>
        /// Facilitates testing without calling Process.Start
        /// </summary>
        public IOAuthHelper OAuthHelper { get; set; }

        public string ConsumerKey 
        {
            get
            {
                return OAuthTwitter.OAuthConsumerKey;
            }
            set
            {
                OAuthTwitter.OAuthConsumerKey = value;
            }
        }

        public string ConsumerSecret
        {
            get
            {
                return OAuthTwitter.OAuthConsumerSecret;
            }
            set
            {
                OAuthTwitter.OAuthConsumerSecret = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return OAuthTwitter.OAuthTokenSecret;
            }
            set
            {
                OAuthTwitter.OAuthTokenSecret = value;
            }
        }

        public bool IsAuthorized { get; protected set; }

        public string UserId { get; protected set; }

        public string ScreenName { get; protected set; }

        public TimeSpan ReadWriteTimeout { get; set; }

        public TimeSpan Timeout { get; set; }

        public string UserAgent { get; set; }

        public bool UseCompression { get; set; }

        private string PrepareAuthHeader(string authHeader)
        {
            var encodedParams =
                string.Join(
                    ",",
                    (from param in authHeader.Split('&')
                     let args = param.Split('=')
                     where !args[0].Contains("realm")
                     select args[0] + "=\"" + args[1] + "\"")
                    .ToArray());

            return "OAuth " + encodedParams;
        }

        /// <summary>
        /// Initializes the request in ways common to GET and POST requests.
        /// </summary>
        /// <param name="request">The request to initialize.</param>
        protected void InitializeRequest(HttpWebRequest request)
        {
            request.UserAgent = UserAgent;

            if (this.ReadWriteTimeout > TimeSpan.Zero)
            {
                request.ReadWriteTimeout = (int)ReadWriteTimeout.TotalMilliseconds;
            }

            if (this.Timeout > TimeSpan.Zero)
            {
                request.Timeout = (int)Timeout.TotalMilliseconds;
            }

            if (this.UseCompression)
            {
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }
        }

        /// <summary>
        /// OAuth Get
        /// </summary>
        /// <param name="url">Twitter Query</param>
        /// <returns>Request to be sent to Twitter</returns>
        public HttpWebRequest Get(string url)
        {
            string outUrl;
            string queryString;
            OAuthTwitter.GetOAuthQueryString(HttpMethod.GET, url, string.Empty, out outUrl, out queryString);

            var req = HttpWebRequest.Create(url) as HttpWebRequest;

            req.Headers.Add(
                HttpRequestHeader.Authorization,
                PrepareAuthHeader(queryString));

            InitializeRequest(req);

            return req;
        }

        public HttpWebRequest Post(string url)
        {
            var req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = HttpMethod.POST.ToString();

            req.Headers.Add(
                HttpRequestHeader.Authorization,
                OAuthTwitter.GetOAuthQueryStringForPost(url));

            InitializeRequest(req);

            return req;
        }

        /// <summary>
        /// OAuth Post
        /// </summary>
        /// <param name="url">Twitter Command</param>
        /// <param name="args">Command Arguments</param>
        /// <returns>Response from Twitter</returns>
        public HttpWebResponse Post(string url, Dictionary<string, string> args)
        {
            string paramsJoined =
                string.Join(
                    "&",
                    (from param in args
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + OAuthTwitter.TwitterParameterUrlEncode(param.Value))
                    .ToArray());

            url += "?" + paramsJoined;

            var req = WebRequest.Create(url) as HttpWebRequest;
            req.ServicePoint.Expect100Continue = false;
            req.Method = HttpMethod.POST.ToString();

            req.Headers.Add
                (HttpRequestHeader.Authorization,
                OAuthTwitter.GetOAuthQueryStringForPost(url));
            req.ContentLength = 0;

            InitializeRequest(req);

            return OAuthHelper.GetResponse(req);
        }
    }
}
