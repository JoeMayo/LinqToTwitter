using System;
using System.Linq;
using System.Threading;

namespace LinqToTwitter
{
    public class PinAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// PIN-based authorization requires a 7-digit pin that is provided by Twitter.
        /// The user must copy that PIN and give it back to the program to use as a verifier
        /// in getting the final access token from Twitter.  You should write code (a lambda)
        /// that allows the user to provide this pin that this code will return.
        /// </summary>
        public Func<string> GetPin { get; set; }

        /// <summary>
        /// Action to redirect user to Twitter authorization page
        /// </summary>
        public Action<string> GoToTwitterAuthorization { get; set; }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public void Authorize()
        {
            if (IsAuthorized) return;

            if (GetPin == null)
            {
                throw new InvalidOperationException("GetPin must have a handler before calling Authorize.");
            }

            if (GoToTwitterAuthorization == null)
            {
                throw new InvalidOperationException("GoToTwitterAuthorization must have a handler before calling Authorize.");
            }

            string link = OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, "oob", false);
            GoToTwitterAuthorization(link);

            string verifier = GetPin();

            // TODO: Refactor to share similar logic with WebAuthorizer
            string oAuthToken =
                (from nameValPair in new Uri(link).Query.TrimStart('?').Split('&')
                 let pair = nameValPair.Split('=')
                 where pair[0] == "oauth_token"
                 select pair[1])
                .SingleOrDefault();

            string screenName;
            string userID;
            OAuthTwitter.AccessTokenGet(oAuthToken, verifier, OAuthAccessTokenUrl, string.Empty, out screenName, out userID);

            ScreenName = screenName;
            UserId = userID;

            Credentials.OAuthToken = OAuthTwitter.OAuthToken;
            Credentials.AccessToken = OAuthTwitter.OAuthTokenSecret;
        }

        /// <summary>
        /// Asynchronously starts the authorization process
        /// </summary>
        /// <param name="authorizationCompleteCallback">Action you provide for when authorization completes.</param>
        public void BeginAuthorize(Action<TwitterAsyncResponse<object>> authorizationCompleteCallback)
        {
            if (IsAuthorized) return;

            if (GoToTwitterAuthorization == null)
            {
                throw new InvalidOperationException("GoToTwitterAuthorization must have a handler before calling BeginAuthorize.");
            }

            OAuthTwitter.GetRequestTokenAsync(new Uri(OAuthRequestTokenUrl), new Uri(OAuthAuthorizeUrl), "oob", false, GoToTwitterAuthorization, authorizationCompleteCallback);
        }

        /// <summary>
        /// Asynchronously finishes the authorization process
        /// </summary>
        /// <param name="pin">Set this to the 7-digit PIN that Twitter provides after the user authorizes your application</param>
        /// <param name="authorizationCompleteCallback">Action you provide for when authorization completes.</param>
        public void CompleteAuthorize(string pin, Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback)
        {
            if (IsAuthorized) return;

            if (pin == null)
            {
                throw new ArgumentNullException("pin", "pin is required");
            }

            OAuthTwitter.GetAccessTokenAsync(pin, new Uri(OAuthAccessTokenUrl), "oob", authorizationCompleteCallback);
        }
    }
}
