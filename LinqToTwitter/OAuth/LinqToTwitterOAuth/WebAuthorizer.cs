using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Net;

namespace LinqToTwitter
{
    [Serializable]
    public class WebAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Perform authorization
        /// </summary>
        public void Authorize()
        {
            if (IsAuthorized) return;

            BeginAuthorization();

            CompleteAuthorization();
        }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public void BeginAuthorization()
        {
            if (IsAuthorized) return;

            string callback = OAuthHelper.GetRequestUrl();
            string link = OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, callback, false, false);

            OAuthHelper.PerformRedirect(link);
        }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public void CompleteAuthorization()
        {
            if (IsAuthorized) return;

            string verifier = OAuthHelper.GetRequestParam("oauth_verifier");

            if (verifier != null)
            {
                string oAuthToken = OAuthHelper.GetRequestParam("oauth_token");

                string screenName;
                string userID;
                OAuthTwitter.AccessTokenGet(oAuthToken, verifier, OAuthAccessTokenUrl, string.Empty, out screenName, out userID);

                ScreenName = screenName;
                UserId = userID;

                IsAuthorized = true;
            }
        }
    }
}
