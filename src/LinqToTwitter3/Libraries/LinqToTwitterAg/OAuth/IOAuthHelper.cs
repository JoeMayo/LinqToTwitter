using System.Net;

namespace LinqToTwitter
{
    public interface IOAuthHelper
    {
        ///// <summary>
        ///// Encapsulates Process.Start so tests don't launch browser
        ///// </summary>
        ///// <param name="url">Address for browser to navigate to</param>
        //void LaunchBrowser(string url);

        /// <summary>
        /// Encapsulates GetResponse so tests don't invoke the request
        /// </summary>
        /// <param name="req">Request to Twitter</param>
        /// <returns>Response to Twitter</returns>
        HttpWebResponse GetResponse(HttpWebRequest req);

        ///// <summary>
        ///// Encapsulates browser redirect so tests don't execute this code
        ///// </summary>
        ///// <param name="url">URL to redirect to</param>
        //void PerformRedirect(string url);

        ///// <summary>
        ///// Pulls request URL without oauth params
        ///// </summary>
        ///// <returns>Reqsuest Url</returns>
        //string GetRequestUrl();

        ///// <summary>
        ///// Gets a parameter from an HttpRequest
        ///// </summary>
        ///// <param name="reqParam">Parameter to get</param>
        ///// <returns>Parameter value</returns>
        //string GetRequestParam(string reqParam);
    }
}
