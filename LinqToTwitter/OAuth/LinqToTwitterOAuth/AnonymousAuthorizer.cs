using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace LinqToTwitter
{
    public class AnonymousAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
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
        public new HttpWebRequest Get(string url)
        {
            Uri requestUri = new Uri(url);
            var req = WebRequest.Create(requestUri) as HttpWebRequest;
            this.InitializeRequest(req);
            return req;
        }

        /// <summary>
        /// Posts the specified request URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <returns></returns>
        public new HttpWebRequest Post(string url)
        {
            Uri requestUri = new Uri(url);
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
        public new HttpWebResponse Post(string url, Dictionary<string, string> args)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            Uri requestUri = new Uri(url);

            string queryString = Utilities.BuildQueryString(args);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);

            var req = WebRequest.Create(requestUri) as HttpWebRequest;

            this.InitializeRequest(req);

            req.Method = HttpMethod.POST.ToString();
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "x-www-form-urlencoded";
            req.ContentLength = queryStringBytes.Length;

            using (Stream requestStream = req.GetRequestStream())
            {
                requestStream.Write(queryStringBytes, 0, queryStringBytes.Length);
            }

            return req.GetResponse() as HttpWebResponse;
        }
    }
}
