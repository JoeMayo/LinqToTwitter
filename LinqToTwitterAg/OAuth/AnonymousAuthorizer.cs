using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
#if SILVERLIGHT
using System.Windows;
#else
#endif

namespace LinqToTwitter
{
    public class AnonymousAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        [Obsolete("Twitter API v1.1 requires all queries to be authorized. Please visit http://linqtotwitter.codeplex.com/wikipage?title=Securing%20Your%20Applications for more guidance on how to use OAuth in your application.", true)]
        public void Authorize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the specified request URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public new WebRequest Get(Request request)
        {
            var url = request.Endpoint;
            var queryString = request.QueryString;

#if SILVERLIGHT
            var fullUrl = 
                (ProxyUrl + url + 
                 (string.IsNullOrEmpty(ProxyUrl) ? "?" : "&") +
                 queryString)
                .Trim('?', '&');

            var requestUri = new Uri(fullUrl);
#else
            var requestUri = new Uri(url + "?" + queryString);
#endif
            var req = WebRequest.Create(requestUri);
            this.InitializeRequest(req);

            return req;
        }

        /// <summary>
        /// Posts the specified request URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <returns></returns>
        public override HttpWebRequest PostRequest(Request request, IDictionary<string, string> postData)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var url = request.Endpoint;

#if SILVERLIGHT
            url = ProxyUrl + url;
#endif
            var requestUri = new Uri(url);
            var req = WebRequest.Create(requestUri) as HttpWebRequest;
            this.InitializeRequest(req);
            req.Method = HttpMethod.POST.ToString();
            return req;
        }

        /// <summary>
        /// Posts the specified request URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public override HttpWebResponse Post(Request request, IDictionary<string, string> postData)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var url = request.Endpoint;

#if SILVERLIGHT
             url = ProxyUrl + url;
#endif
            var requestUri = new Uri(url);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(request.QueryString);

            var req = WebRequest.Create(requestUri) as HttpWebRequest;

            this.InitializeRequest(req);

            req.Method = HttpMethod.POST.ToString();
            //req.ServicePoint.Expect100Continue = false;
            req.Headers[HttpRequestHeader.Expect] = null;
            req.ContentType = "x-www-form-urlencoded";
#if !WINDOWS_PHONE && !NETFX_CORE
            req.ContentLength = queryStringBytes.Length; 
#endif

            var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false);

            req.BeginGetRequestStream(
                new AsyncCallback(
                    ar =>
                    {
                        using (var requestStream = req.EndGetRequestStream(ar))
                        {
                            requestStream.Write(queryStringBytes, 0, queryStringBytes.Length);
                        }
                        resetEvent.Set();
                    }), null);

            resetEvent.WaitOne();
            resetEvent.Reset();

            HttpWebResponse res = null;

            req.BeginGetResponse(
                new AsyncCallback(
                    ar =>
                    {
                        res = req.EndGetResponse(ar) as HttpWebResponse;
                        resetEvent.Set();
                    }), null);

            resetEvent.WaitOne();

            return res;
        }
    }
}
