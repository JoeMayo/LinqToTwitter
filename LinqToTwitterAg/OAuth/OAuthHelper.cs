using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace LinqToTwitter
{
    /// <summary>
    /// Methods to facilitate testing and other utilities
    /// </summary>
    public class OAuthHelper : IOAuthHelper
    {
        ///// <summary>
        ///// Encapsulates Process.Start so tests don't launch browser
        ///// </summary>
        ///// <param name="url">Address for browser to navigate to</param>
        //public void LaunchBrowser(string url)
        //{
        //    Process.Start(url);
        //}

        /// <summary>
        /// Encapsulates GetResponse so tests don't invoke the request
        /// </summary>
        /// <param name="req">Request to Twitter</param>
        /// <returns>Response to Twitter</returns>
        public HttpWebResponse GetResponse(HttpWebRequest req)
        {
            var resetEvent = new ManualResetEvent(initialState: false);
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

        ///// <summary>
        ///// Encapsulates browser redirect so tests don't execute this code
        ///// </summary>
        ///// <param name="url">URL to redirect to</param>
        //public void PerformRedirect(string url)
        //{
        //    HttpContext.Current.Response.Redirect(url);
        //}

        ///// <summary>
        ///// Pulls request URL without oauth params
        ///// </summary>
        ///// <returns>Reqsuest Url</returns>
        //public string GetRequestUrl()
        //{
        //    string fullUrl = HttpContext.Current.Request.Url.ToString();
        //    string[] urlParts = fullUrl.Split('?');
        //    string filteredParams = string.Empty;

        //    string url = urlParts[0];

        //    if (urlParts.Length == 2)
        //    {
        //        string urlParams = urlParts[1];

        //        filteredParams =
        //            string.Join(
        //                "&",
        //                (from param in urlParams.Split('&')
        //                 let args = param.Split('=')
        //                 where !args[0].StartsWith("oauth_")
        //                 select param)
        //                .ToArray()); 
        //    }

        //    return url + (filteredParams == string.Empty ? string.Empty : "?" + filteredParams);
        //}

        ///// <summary>
        ///// Gets a parameter from an HttpRequest
        ///// </summary>
        ///// <param name="reqParam">Parameter to get</param>
        ///// <returns>Parameter value</returns>
        //public string GetRequestParam(string reqParam)
        //{
        //    return HttpContext.Current.Request.Params[reqParam];
        //}
    }
}
