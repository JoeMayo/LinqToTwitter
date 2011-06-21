using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public class XAuthAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Synchronous authorization
        /// </summary>
        public void Authorize()
        {
            if (IsAuthorized)
                return;

            var request = new Request(OAuthAccessTokenUrl);

            var xauthCredentials = Credentials as XAuthCredentials;
            var postData = new Dictionary<string, string>();
            postData.Add("x_auth_username", xauthCredentials.UserName);
            postData.Add("x_auth_password", xauthCredentials.Password);
            postData.Add("x_auth_mode", "client_auth");

            string screenName;
            string userID;
            OAuthTwitter.PostAccessToken(request, postData, out screenName, out userID);

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
            if (IsAuthorized)
                return;

            var request = new Request(OAuthAccessTokenUrl);

            var xauthCredentials = Credentials as XAuthCredentials;
            var postData = new Dictionary<string, string>();
            postData.Add("x_auth_username", xauthCredentials.UserName);
            postData.Add("x_auth_password", xauthCredentials.Password);
            postData.Add("x_auth_mode", "client_auth");

            OAuthTwitter.PostAccessTokenAsync(request, postData, authorizationCompleteCallback);
        }
    }
}
