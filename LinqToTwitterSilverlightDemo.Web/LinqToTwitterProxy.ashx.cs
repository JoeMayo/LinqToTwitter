/***********************************************************
 * Credits:
 * 
 * Based On: Peter Bromberg's Silverlight Multipurpose WebRequest Proxy:
 *     http://eggheadcafe.com/tutorials/aspnet/45d1dc2b-7db7-4bbc-b612-003fe889ed4a/silverlight-multipurpose.aspx
 *     
 ***********************************************************/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;

namespace LinqToTwitterSilverlightDemo.Web
{
    /// <summary>
    /// Proxy for Silverlight Web calls
    /// </summary>
    public class LinqToTwitterProxy : IHttpHandler
    {
        /// <summary>
        /// Handle proxy request to Twitter
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            string url = PrepareUrl(context);

            if (context.Request.HttpMethod == "GET")
            {
                var client = new WebClient();
                client = SetCookies(client, context.Request);
                string s = client.DownloadString(url);
                context.Response.Write(s);
                client.Dispose();
            }

            if (context.Request.HttpMethod == "POST")
            {
                var req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = "POST";
                req = SetCookies(req, context.Request);
                req = SetHeaders(req, context.Request.Headers);
                string postData =
                    string.Join(
                        "&",
                        (from string key in context.Request.Form
                         select key + "=" + context.Request.Form[key])
                        .ToArray());

                byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);
                using (var reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                    reqStream.Flush();
                }

                using (WebResponse resp = req.GetResponse())
                {
                    using (var respStream = resp.GetResponseStream())
                    using (var respReader = new StreamReader(respStream))
                    {
                        string respString = respReader.ReadToEnd();
                        context.Response.Write(respString);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures url is properly encoded and formatted
        /// </summary>
        /// <param name="context">HttpContext for accessing Request</param>
        /// <returns>URL for query</returns>
        private string PrepareUrl(HttpContext context)
        {
            var queryDict =
                (from keyValPair in context.Request.QueryString.ToString().Split('&')
                 let pairArr = keyValPair.Split('=')
                 let key = pairArr[0]
                 let val = pairArr[1]
                 select new
                 {
                     Key = key,
                     Val = context.Request.QueryString[key]
                 })
                .ToDictionary(pair => pair.Key, pair => pair.Val);

            string queryString = string.Join("&",
                (from key in queryDict.Keys
                 where key != "url"
                 select key + "=" + HttpUtility.UrlEncode(queryDict[key]))
                .ToArray());

            string url = queryDict["url"];
            if (queryString.Length > 0)
            {
                url += "?" + queryString;
            }

            return url;
        }

        /// <summary>
        /// Transfer any cookies from original Silverlight request to real outbound request
        /// </summary>
        /// <param name="wc">WebClient with Headers property</param>
        /// <param name="request">Request containing original cookies</param>
        /// <returns>WebClient with updated cookies</returns>
        private WebClient SetCookies(WebClient wc, HttpRequest request)
        {
            HttpCookieCollection coll = request.Cookies;
            foreach (string cookieName in coll.Keys)
            {
                wc.Headers.Add("Cookie", cookieName + "=" + coll[cookieName].Value);
            }
            return wc;
        }

        /// <summary>
        /// Transfer any cookies from original Silverlight request to real outbound request
        /// </summary>
        /// <param name="wc">HttpRequest with Headers property</param>
        /// <param name="origReq">Request containing original cookies</param>
        /// <returns>HttpRequest with updated cookies</returns>
        private HttpWebRequest SetCookies(HttpWebRequest proxyReq, HttpRequest origReq)
        {
            HttpCookieCollection coll = origReq.Cookies;
            foreach (string cookieName in coll.Keys)
            {
                proxyReq.Headers.Add("Cookie", cookieName + "=" + coll[cookieName].Value);
            }
            return proxyReq;
        }

        /// <summary>
        /// Transfer Headers for Twitter query
        /// </summary>
        /// <param name="proxyReq">WebClient with Headers property</param>
        /// <param name="origReq">Headers, from original request, to transfer</param>
        /// <returns>WebClient with updated headers</returns>
        private HttpWebRequest SetHeaders(HttpWebRequest proxyReq, NameValueCollection origReq)
        {
            var headersToNotSet = new List<string> 
            { 
                "Accept",
                "Accept-Encoding",
                "Authorization", 
                "Connection", 
                "Content-Length", 
                "Content-Type",
                "Host", 
                "Referer",
                "User-Agent"
            };
            string hdrvalue = "";

            foreach (string headerName in origReq.Keys)
            {
                hdrvalue = origReq[headerName];

                if (!headersToNotSet.Contains(headerName))
                    proxyReq.Headers.Add(headerName, hdrvalue);
            }

            if (origReq["Authorization"] != null)
            {
                proxyReq.Headers["Authorization"] = origReq["Authorization"]; 
            }

            proxyReq.ServicePoint.Expect100Continue = false;

            if (origReq["User-Agent"] != null)
            {
                proxyReq.UserAgent = origReq["User-Agent"];
            }

            return proxyReq;
        }

        /// <summary>
        /// This handler can't be reused
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }
    }
}