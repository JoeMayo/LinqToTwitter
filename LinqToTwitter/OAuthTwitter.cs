/***********************************************************
 * Credits:
 * 
 * Eran Sandler -
 * OAuthBase Class
 * 
 * http://oauth.googlecode.com/svn/code/csharp/
 * 
 * Shannon Whitley -
 * Example of how to use modified version of
 * Eran Sandler's OAuthBase class in C#
 * 
 * http://www.voiceoftech.com/swhitley/?p=681
 * 
 * Joe Mayo -
 * 
 * Modified 5/17/09
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.IO;

namespace LinqToTwitter
{
    /// <summary>
    /// helps perform OAuth Authorization for LINQ to Twitter
    /// </summary>
    public class OAuthTwitter : OAuthBase, IOAuthTwitter
    {
        /// <summary>
        /// user agent header sent to Twitter
        /// </summary>
        public string OAuthUserAgent { get; set; }

        #region Properties

        /// <summary>
        /// Consumer Key
        /// </summary>
        public string OAuthConsumerKey { get; set; }

        /// <summary>
        /// Consumer Secret
        /// </summary>
        public string OAuthConsumerSecret { get; set; }

        /// <summary>
        /// OAuth Token
        /// </summary>
        public string OAuthToken  { get; set; }
        
        /// <summary>
        /// OAuth Token Secret
        /// </summary>
        public string OAuthTokenSecret  { get; set; }

        #endregion

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <param name="readOnly">true for read-only, otherwise read/Write</param>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet(string requestToken, string authorizeUrl, bool readOnly, bool forceLogin)
        {
            string ret = null;
            string response = oAuthWebRequest(HttpMethod.GET, requestToken, String.Empty);
            if (response.Length > 0)
            {
                var prefixChar = "?";

                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    ret = authorizeUrl + "?oauth_token=" + qs["oauth_token"];
                    prefixChar = "&";
                }

                if (readOnly)
                {
                    ret += prefixChar + "oauth_access_type=read";
                    prefixChar = "&";
                }

                if (readOnly)
                {
                    ret += prefixChar + "force_login=true";
                }
            }
            return ret;
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken, string accessTokenUrl, out string screenName, out string userID)
        {
            this.OAuthToken = authToken;
            screenName = string.Empty;
            userID = string.Empty;

            string response = oAuthWebRequest(HttpMethod.GET, accessTokenUrl, String.Empty);

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);

                if (qs["oauth_token"] != null)
                {
                    this.OAuthToken = qs["oauth_token"];
                }

                if (qs["oauth_token_secret"] != null)
                {
                    this.OAuthTokenSecret = qs["oauth_token_secret"];
                }

                if (qs["screen_name"] != null)
                {
                    screenName = qs["screen_name"];
                }

                if (qs["user_id"] != null)
                {
                    userID = qs["user_id"];
                }
            }
        }

        /// <summary>
        /// returns a query string for an OAuth request
        /// </summary>
        /// <param name="url">Twitter query</param>
        /// <returns>Query string with OAuth parameters</returns>
        public void GetOAuthQueryString(HttpMethod method, string url, out string outUrl, out string queryString)
        {
            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                this.OAuthConsumerKey,
                this.OAuthConsumerSecret,
                this.OAuthToken,
                this.OAuthTokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out queryString);

            queryString += "&oauth_signature=" + HttpUtility.UrlEncode(sig);
        }

        /// <summary>
        /// processes POST request parameters
        /// </summary>
        /// <param name="parameters">parameters to process</param>
        public void GetOAuthQueryStringForPost(string url, Dictionary<string, string> parameters, out string outUrl, out string postData)
        {
            var paramString = string.Empty;

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (string.IsNullOrEmpty(parameters[key]))
                    {
                        continue;
                    }
                    if (paramString.Length > 0)
                    {
                        paramString += "&";
                    }
                    var param = string.Empty;
                    param = HttpUtility.UrlDecode(parameters[key]);
                    param = this.UrlEncode(param);
                    paramString += key + "=" + param;
                } 
            }

            if (url.IndexOf("?") > 0)
            {
                url += "&";
            }
            else
            {
                url += "?";
            }

            GetOAuthQueryString(HttpMethod.POST, url + paramString, out outUrl, out postData);
        }

        /// <summary>
        /// processes POST request parameters
        /// </summary>
        /// <param name="parameters">parameters to process</param>
        public string GetOAuthAuthorizationHeader(string url, Dictionary<string, string> parameters)
        {
            const int Key = 0;
            const int Value = 1;

            string outUrl;
            string postData;

            GetOAuthQueryStringForPost(url, parameters, out outUrl, out postData);

            var headerItems =
                from param in postData.Split('&')
                let keyValPair = param.Split('=')
                select
                    keyValPair[Key] +
                    "=\"" +
                    keyValPair[Value] +
                    "\"";

            return "OAuth realm=\"" + outUrl + "\"," + string.Join(",", headerItems.ToArray());
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string oAuthWebRequest(HttpMethod method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";

            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == HttpMethod.POST)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                this.OAuthConsumerKey,
                this.OAuthConsumerSecret,
                this.OAuthToken,
                this.OAuthTokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            //Convert the querystring to postData
            if (method == HttpMethod.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            ret = WebRequest(method, outUrl + querystring, postData);

            return ret;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(HttpMethod method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent  = OAuthUserAgent;

            if (method == HttpMethod.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}
