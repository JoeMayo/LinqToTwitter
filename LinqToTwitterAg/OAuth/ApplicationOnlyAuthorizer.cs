using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using LinqToTwitter.Common;
using LinqToTwitter.Security.Application;
using LitJson;

namespace LinqToTwitter
{
    public class ApplicationOnlyAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        public string BasicToken { get; set; }
        public string BearerToken { get; set; }
        public string OAuth2Token { get; set; }
        public string OAuth2InvalidateToken { get; set; }

        public ApplicationOnlyAuthorizer()
        {
            OAuth2Token = "https://api.twitter.com/oauth2/token";
            OAuth2InvalidateToken = "https://api.twitter.com/oauth2/invalidate_token";
        }

        public void Authorize()
        {
            EncodeCredentials();
            GetBearerToken();
        }

        public override bool IsAuthorized
        {
            get
            {
                return !string.IsNullOrEmpty(BearerToken);
            }
        }

        public void Invalidate()
        {
            EncodeCredentials();
            string url = OAuth2InvalidateToken;
#if SILVERLIGHT
            url = ProxyUrl + OAuth2InvalidateToken;
#endif

            var req = WebRequest.Create(url) as HttpWebRequest;

            req.Method = HttpMethod.POST.ToString();
            req.Headers[HttpRequestHeader.Authorization] = "Basic " + BasicToken; ;
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("access_token=" + BearerToken);
#if !WINDOWS_PHONE && !NETFX_CORE
            req.UserAgent = UserAgent;
            req.ContentLength = data.Length;
#endif

            string response = null;

            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
            {
                req.BeginGetRequestStream(
                    ar =>
                    {
                        using (var reqStream = req.EndGetRequestStream(ar))
                            reqStream.Write(data, 0, data.Length);

                        req.BeginGetResponse(
                            ar2 =>
                            {
                                var resp = req.EndGetResponse(ar2);
                                response = resp.ReadReponse();
#if !WINDOWS_PHONE && !NETFX_CORE
                                resetEvent.Set();
#endif
                            },
                            null);
                    },
                    null);
#if !WINDOWS_PHONE && !NETFX_CORE
                resetEvent.WaitOne();
#endif

                if (response != null)
                {
                    var responseJson = JsonMapper.ToObject(response);
                    BearerToken = responseJson.GetValue<string>("access_token");
                }
            }
        }
  
        void GetBearerToken()
        {
            string url = OAuth2Token;
#if SILVERLIGHT
            url = ProxyUrl + OAuth2Token;
#endif

            var req = WebRequest.Create(url) as HttpWebRequest;

            req.Method = HttpMethod.POST.ToString();
            req.Headers[HttpRequestHeader.Authorization] = "Basic " + BasicToken;
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("grant_type=client_credentials");
#if !WINDOWS_PHONE && !NETFX_CORE
            req.UserAgent = UserAgent;
            req.ContentLength = data.Length;
#endif

            string response = null;

            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
            {
                Exception thrownException = null;

                req.BeginGetRequestStream(
                    ar =>
                    {
                        using (var reqStream = req.EndGetRequestStream(ar))
                            reqStream.Write(data, 0, data.Length);

                        req.BeginGetResponse(
                            ar2 =>
                            {
                                try
                                {
                                    var resp = req.EndGetResponse(ar2);
                                    response = resp.ReadReponse();
                                }
                                catch (Exception ex)
                                {
                                    thrownException = ex;
                                }
                                finally
                                {
#if !WINDOWS_PHONE && !NETFX_CORE
                                    resetEvent.Set();
#endif
                                }
                            },
                            null);
                    },
                    null);
#if !WINDOWS_PHONE && !NETFX_CORE
                resetEvent.WaitOne();
#endif
                if (thrownException != null) throw thrownException;

                if (response != null)
                {
                    var responseJson = JsonMapper.ToObject(response);
                    BearerToken = responseJson.GetValue<string>("access_token");
                }
            }

        }

        internal void EncodeCredentials()
        {
            string encodedConsumerKey = Encoder.UrlEncode(Credentials.ConsumerKey);
            string encodedConsumerSecret = Encoder.UrlEncode(Credentials.ConsumerSecret);

            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;

            byte[] credBytes = System.Text.Encoding.UTF8.GetBytes(concatenatedCredentials);

            string base64Credentials = Convert.ToBase64String(credBytes);

            BasicToken = base64Credentials;
        }

        /// <summary>
        /// OAuth Get
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>Request to be sent to Twitter</returns>
        public new WebRequest Get(Request request)
        {
#if SILVERLIGHT
            var fullUrl = ProxyUrl + request.FullUrl;

            var req = WebRequest.Create(fullUrl) as HttpWebRequest;
            req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken;
#else
            var req = WebRequest.Create(request.FullUrl) as HttpWebRequest;
            if (req != null)
            {
                req.Headers[HttpRequestHeader.Authorization] =  "Bearer " + BearerToken;

                InitializeRequest(req);
            }
#endif

            return req;
        }

        /// <summary>
        /// OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>request to send</returns>
        public override HttpWebRequest PostRequest(Request request, IDictionary<string, string> postData)
        {
#if SILVERLIGHT
            var req = HttpWebRequest.Create(
                ProxyUrl + request.Endpoint +
                (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                request.QueryString) as HttpWebRequest;
#else
            var req = WebRequest.Create(request.FullUrl) as HttpWebRequest;
#endif

            if (req != null)
            {
#if !SILVERLIGHT && !NETFX_CORE
                req.ServicePoint.Expect100Continue = false;
#endif
                req.Method = HttpMethod.POST.ToString();
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken;
#if !WINDOWS_PHONE && !NETFX_CORE
                req.ContentLength = 0;
#endif

                InitializeRequest(req);
            }

            return req;
        }

        /// <summary>
        /// OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>request to send</returns>
        public override HttpWebResponse Post(Request request, IDictionary<string, string> postData)
        {
            var req = PostRequest(request, postData);
            return Utilities.AsyncGetResponse(req);
        }

        /// <summary>
        /// Async OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Hash of parameters</param>
        /// <returns>HttpWebRequest for post</returns>
        public override HttpWebRequest PostAsync(Request request, IDictionary<string, string> postData)
        {
            var req = WebRequest.Create(
                    ProxyUrl + request.Endpoint +
                    (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                    request.QueryString)
                as HttpWebRequest;

            if (req != null)
            {
                req.Method = HttpMethod.POST.ToString();
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + BearerToken; ;
#if !WINDOWS_PHONE && !NETFX_CORE
                req.ContentLength = 0;
#endif

                InitializeRequest(req);
            }

            return req;
        }

    }
}
