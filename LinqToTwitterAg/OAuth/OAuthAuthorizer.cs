using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

#if SILVERLIGHT
using System.Net.Browser;
using System.Windows;
#endif

namespace LinqToTwitter
{
    public abstract class OAuthAuthorizer
    {
        public OAuthAuthorizer()
        {
            OAuthRequestTokenUrl = "https://api.twitter.com/oauth/request_token";
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authorize";
            OAuthAccessTokenUrl = "https://api.twitter.com/oauth/access_token";

            OAuthTwitter = new OAuthTwitter();
            OAuthHelper = new OAuthHelper();

#if SILVERLIGHT
            ProxyUrl =
                Application.Current.Host.Source.Scheme + "://" +
                Application.Current.Host.Source.Host + ":" +
                Application.Current.Host.Source.Port + "/LinqToTwitterProxy.ashx?url=";
#else
            ProxyUrl = string.Empty;
#endif
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
        /// URL for Silverlight proxy
        /// </summary>
        public string ProxyUrl 
        {
            get
            {
                return OAuthTwitter.ProxyUrl;
            }
            set
            {
                OAuthTwitter.ProxyUrl = value;
            }
        }

        /// <summary>
        /// Contains general OAuth functionality
        /// </summary>
        public IOAuthTwitter OAuthTwitter { get; set; }

        /// <summary>
        /// Facilitates testing without calling Process.Start
        /// </summary>
        public IOAuthHelper OAuthHelper { get; set; }

        private IOAuthCredentials m_credentials;

        /// <summary>
        /// Holds ConsumerKey, ConsumerSecret, and AccessToken
        /// 
        /// Note: Populate Credentials before setting this property
        /// </summary>
        public IOAuthCredentials Credentials
        {
            get
            {
                return m_credentials;
            }
            set
            {
                m_credentials = value;
                OAuthTwitter.OAuthConsumerKey = value.ConsumerKey;
                OAuthTwitter.OAuthConsumerSecret = value.ConsumerSecret;
                OAuthTwitter.OAuthToken = value.OAuthToken;
                OAuthTwitter.OAuthTokenSecret = value.AccessToken;
            }
        }

        public bool IsAuthorized 
        {
            get
            {
                if (Credentials == null)
                {
                    throw new ArgumentNullException("Credentials", "You must set the Credentials property.");
                }

                return
                    !string.IsNullOrEmpty(Credentials.ConsumerKey) &&
                    !string.IsNullOrEmpty(Credentials.ConsumerSecret) &&
                    !string.IsNullOrEmpty(Credentials.OAuthToken) &&
                    !string.IsNullOrEmpty(Credentials.AccessToken);
            }
        }

        public string UserId { get; set; }

        public string ScreenName { get; set; }

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
        /// <param name="webRequest">The request to initialize.</param>
        protected void InitializeRequest(WebRequest webRequest)
        {
#if !SILVERLIGHT
            var request = webRequest as HttpWebRequest;

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
#endif
        }

        /// <summary>
        /// OAuth Get
        /// </summary>
        /// <param name="url">Twitter Query</param>
        /// <returns>Request to be sent to Twitter</returns>
        public WebRequest Get(string url)
        {
            string outUrl;
            string queryString;
            OAuthTwitter.GetOAuthQueryString(HttpMethod.GET, url, string.Empty, out outUrl, out queryString);

#if SILVERLIGHT
            var req = HttpWebRequest.Create(
                ProxyUrl + url + 
                (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                queryString);
#else
            var req = HttpWebRequest.Create(outUrl + "?" + queryString) as HttpWebRequest;
            //req.Headers[HttpRequestHeader.Authorization] = PrepareAuthHeader(queryString);

            InitializeRequest(req);
#endif

            return req;
        }

        public HttpWebRequest Post(string url)
        {
            var req = WebRequest.Create(ProxyUrl + url) as HttpWebRequest;
            req.Method = HttpMethod.POST.ToString();

            req.Headers[HttpRequestHeader.Authorization] = OAuthTwitter.GetOAuthQueryStringForPost(url);

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

            var req = WebRequest.Create(ProxyUrl + url) as HttpWebRequest;
            //req.ServicePoint.Expect100Continue = false;
            req.Method = HttpMethod.POST.ToString();

            req.Headers[HttpRequestHeader.Authorization] = OAuthTwitter.GetOAuthQueryStringForPost(url);
            req.ContentLength = 0;

            InitializeRequest(req);

            return OAuthHelper.GetResponse(req);
        }

        /// <summary>
        /// Async OAuth Post
        /// </summary>
        /// <param name="url">Twitter Command</param>
        /// <param name="args">Command Arguments</param>
        /// <returns>HttpWebRequest for post</returns>
        public HttpWebRequest PostAsync(string url, Dictionary<string, string> args)
        {
            string paramsJoined =
                string.Join(
                    "&",
                    (from param in args
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + OAuthTwitter.TwitterParameterUrlEncode(param.Value))
                    .ToArray());

            //url += "?" + paramsJoined;

            var req = WebRequest.Create(
                    ProxyUrl + url +
                    (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                    paramsJoined) 
                as HttpWebRequest;
            //req.ServicePoint.Expect100Continue = false;
            req.Method = HttpMethod.POST.ToString();

            req.Headers[HttpRequestHeader.Authorization] = OAuthTwitter.GetOAuthQueryStringForPost(url + "?" + paramsJoined);
            req.ContentLength = 0;

            InitializeRequest(req);

            return req;
        }
    }
}
