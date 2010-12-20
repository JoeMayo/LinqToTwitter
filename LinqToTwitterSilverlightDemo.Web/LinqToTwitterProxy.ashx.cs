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
                var wc = new WebClient();
                wc = SetCookies(wc, context.Request);
                wc = SetHeaders(wc, context.Request.Headers);
                NameValueCollection nameValueCollection = context.Request.Form;
                byte[] responseArray = wc.UploadValues(url, "POST", nameValueCollection);
                string returnString = Encoding.UTF8.GetString(responseArray);
                context.Response.Write(returnString);
                wc.Dispose();
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
        /// Transfer Headers for Twitter query
        /// </summary>
        /// <param name="wc">WebClient with Headers property</param>
        /// <param name="headers">Headers, from original request, to transfer</param>
        /// <returns>WebClient with updated headers</returns>
        private WebClient SetHeaders(WebClient wc, NameValueCollection headers)
        {
            var headersToNotSet = new List<string> 
            { 
                "Authorization", 
                "Connection", 
                "Content-Length", 
                "Host", 
            };
            string hdrvalue = "";

            foreach (string headerName in headers.Keys)
            {
                hdrvalue = headers[headerName];

                if (!headersToNotSet.Contains(headerName))
                    wc.Headers.Add(headerName, hdrvalue);
            }

            if (headers["Authorization"] != null)
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, headers["Authorization"]); 
            }

            return wc;
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