using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;

namespace LinqToTwitter
{
    /// <summary>
    /// Use this class, or a derivative, in Web applications for OAuth authentication.
    /// </summary>
    public class WebAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public Uri Callback { get; set; }

        /// <summary>
        /// This is a hook where you can assign
        /// a lambda to perform the technology
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

            string callbackStr = OAuthTwitter.FilterRequestParameters(callback);
            string link = OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, callbackStr, readOnly: false, forceLogin: false);

            PerformRedirect(link);
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
            if (callback == null)
            {
                throw new ArgumentNullException("callback", "You must pass in the callback that Twitter returned after authentication.");
            }

            if (IsAuthorized) return true;

            string verifier = OAuthTwitter.GetUrlParamValue(callback.Query, "oauth_verifier");

            if (verifier != null)
            {
                string oAuthToken = OAuthTwitter.GetUrlParamValue(callback.Query, "oauth_token");

                string screenName;
                string userID;
                OAuthTwitter.AccessTokenGet(oAuthToken, verifier, OAuthAccessTokenUrl, string.Empty, out screenName, out userID);

                ScreenName = screenName;
                UserId = userID;

                Credentials.OAuthToken = OAuthTwitter.OAuthToken;
                Credentials.AccessToken = OAuthTwitter.OAuthTokenSecret;
            }

            return IsAuthorized;
        }
    }
}
