using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Net;

namespace LinqToTwitter
{
    /// <summary>
    /// Use this class, or a derivative, in Web applications for OAuth authentication.
    /// </summary>
    [Serializable]
    public class WebAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public Uri Callback { get; set; }

        /// <summary>
        /// Redirection logic differs between Silverlight
        /// and ASP.NET, so this is a hook where you can
        /// attach a lambda to perform the technology
        /// specific redirection action.
        /// 
        /// The string passed as the lambda paramter
        /// is the Twitter authorization URL.
        /// </summary>
        public Action<string> PerformRedirect { get; set; }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public void Authorize()
        {
            if (IsAuthorized) return;

            BeginAuthorization(Callback);

            CompleteAuthorization(Callback);
        }

        /// <summary>
        /// First part of the authorization sequence that:
        /// 1. Obtains a request token and then
        /// 2. Redirects to the Twitter authorization page
        /// </summary>
        /// <param name="callback">This is where you want Twitter to redirect to after authorization</param>
        public void BeginAuthorization(Uri callback)
        {
            if (IsAuthorized) return;

            string callbackStr = FilterRequestParameters(callback);
            string link = OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, callbackStr, readOnly: false, forceLogin: false);

            PerformRedirect(link);
        }


        /// <summary>
        /// Removes OAuth parameters from URL
        /// </summary>
        /// <param name="fullUrl">Raw url with OAuth parameters</param>
        /// <returns>Filtered url without OAuth parameters</returns>
        private static string FilterRequestParameters(Uri fullUrl)
        {
            string filteredParams = string.Empty;

            string url = fullUrl.GetLeftPart(UriPartial.Path);
            string urlParams = fullUrl.Query;

            if (!string.IsNullOrEmpty(urlParams))
            {
                filteredParams =
                    string.Join(
                        "&",
                        (from param in urlParams.Split('&')
                         let args = param.Split('=')
                         where !args[0].StartsWith("oauth_")
                         select param)
                        .ToArray());
            }

            return url + (filteredParams == string.Empty ? string.Empty : "?" + filteredParams);
        }

        /// <summary>
        /// After the user Authorizes the app, Twitter will 
        /// redirect to the callback url, provided during 
        /// BeginAuthorization. When redirecting, Twitter will 
        /// also provide oauth_verifier and oauth_token 
        /// parameters. This method uses those parameters to 
        /// request an access token, which is used automatically
        /// by LINQ to Twitter when executing queries.
        /// </summary>
        /// <param name="callback">URL that Twitter redirected to after authorization</param>
        /// <returns>True if successful</returns>
        public bool CompleteAuthorization(Uri callback)
        {
            if (IsAuthorized) return true;

            string verifier = GetUrlParamValue(callback.Query, "oauth_verifier");

            if (verifier != null)
            {
                string oAuthToken = GetUrlParamValue(callback.Query, "oauth_token");

                string screenName;
                string userID;
                OAuthTwitter.AccessTokenGet(oAuthToken, verifier, OAuthAccessTokenUrl, string.Empty, out screenName, out userID);

                ScreenName = screenName;
                UserId = userID;

                IsAuthorized = true;
            }

            return IsAuthorized;
        }

        /// <summary>
        /// Extracts a value from a query string matching a key
        /// </summary>
        /// <param name="queryString">query string</param>
        /// <param name="paramKey">key to match val</param>
        /// <returns>value matching key</returns>
        public string GetUrlParamValue(string queryString, string paramKey)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }

            string[] keyValPairs = queryString.TrimStart('?').Split('&');
 
            var paramVal =
                (from keyValPair in keyValPairs
                 let pair = keyValPair.Split('=')
                 let key = pair[0]
                 let val = pair[1]
                 where key == paramKey
                 select val)
                .SingleOrDefault();

            return paramVal;
        }
    }
}
