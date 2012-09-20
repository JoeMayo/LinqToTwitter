using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace LinqToTwitter.OAuth
{
    /// <summary>
    /// Supports WinRtAuthenticator by using WebAuthorizationBroker to manage OAuth workflow
    /// </summary>
    /// <remarks>
    /// Will need to be refactored, but was added as a plug-in 
    /// replacement for previously implemented 3rd party library.
    /// </remarks>
    class TwitterAuthProvider : OAuthAuthorizer
    {
        dynamic configuration;

        /// <summary>
        /// Set up configuration information
        /// </summary>
        /// <param name="configuration"></param>
        internal void Configure(dynamic configuration)
        {
            this.configuration = configuration;
            OAuthTwitter.OAuthConsumerKey = configuration.TwitterClientId;
            OAuthTwitter.OAuthConsumerSecret = configuration.TwitterClientSecret;
        }

        /// <summary>
        /// Performs authentication asynchronously, managing entire OAuth workflow
        /// </summary>
        /// <returns>TwitterAuthProviderUser with ScreenName and ID</returns>
        internal async Task<TwitterAuthProviderUser> AuthenticateAsync()
        {
            string callbackStr = OAuthTwitter.FilterRequestParameters(new Uri(configuration.TwitterRedirectUrl));
            string link = OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, callbackStr, false, AuthAccessType);

            WebAuthenticationResult WebAuthenticationResult = 
                await WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None,
                    new Uri(link),
                    new Uri(configuration.TwitterRedirectUrl));

            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var authCallbackUri = new Uri(WebAuthenticationResult.ResponseData);

                string verifier = OAuthTwitter.GetUrlParamValue(authCallbackUri.Query, "oauth_verifier");

                string oAuthToken = OAuthTwitter.GetUrlParamValue(authCallbackUri.Query, "oauth_token");

                string screenName;
                string userID;
                OAuthTwitter.AccessTokenGet(oAuthToken, verifier, OAuthAccessTokenUrl, string.Empty, out screenName, out userID);

                ScreenName = screenName;
                UserId = userID;

                OAuthToken = OAuthTwitter.OAuthToken;
                OAuthTokenSecret = OAuthTwitter.OAuthTokenSecret;

                return new TwitterAuthProviderUser
                {
                    UserName = screenName,
                    Id = userID
                };
            }

            return new TwitterAuthProviderUser
            {
                UserName = "",
                Id = ""
            };
        }

        /// <summary>
        /// Twitter's OAuth token for authenticating user
        /// </summary>
        public string OAuthToken { get; set; }

        /// <summary>
        /// Twitter's Access token for authenticating user
        /// </summary>
        public string OAuthTokenSecret { get; set; }
    }
}
