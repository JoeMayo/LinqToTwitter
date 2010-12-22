
using System;
namespace LinqToTwitter
{
    public class XAuthAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Synchronous authorization
        /// </summary>
        public void Authorize()
        {
            if (IsAuthorized) return;

            var xauthCredentials = Credentials as XAuthCredentials;

            string postData =
                "x_auth_username=" + xauthCredentials.UserName + "&" +
                "x_auth_password=" + xauthCredentials.Password + "&" +
                "x_auth_mode=client_auth";

            string screenName;
            string userID;
            OAuthTwitter.PostAccessToken(OAuthAccessTokenUrl, postData, out screenName, out userID);

            ScreenName = screenName;
            UserId = userID;

            Credentials.OAuthToken = OAuthTwitter.OAuthToken;
            Credentials.AccessToken = OAuthTwitter.OAuthTokenSecret;
        }

        /// <summary>
        /// Asynchronously performs authorization for Silverlight apps
        /// </summary>
        /// <param name="authorizationCompleteCallback">Action you provide for when authorization completes.</param>
        public void BeginAuthorize(Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback)
        {
            if (IsAuthorized) return;

            var xauthCredentials = Credentials as XAuthCredentials;

            string postData =
                "x_auth_username=" + xauthCredentials.UserName + "&" +
                "x_auth_password=" + xauthCredentials.Password + "&" +
                "x_auth_mode=client_auth";

            string url = OAuthAccessTokenUrl + "?" + postData;

            OAuthTwitter.PostAccessTokenAsync(new Uri(url), postData, authorizationCompleteCallback);
        }
    }
}
