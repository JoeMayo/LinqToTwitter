using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Implements the "Sign-in With Twitter" feature
    /// </summary>
    public class SignInAuthorizer : WebAuthorizer, ITwitterAuthorizer
    {
        public SignInAuthorizer()
        {
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authenticate";
        }

        /// <summary>
        /// First part of the authorization sequence that:
        /// 1. Obtains a request token and then
        /// 2. Redirects to the Twitter authorization page
        /// </summary>
        /// <param name="forceLogin">Forces user to login for Sign-In with Twitter scenarios</param>
        /// <param name="callback">This is where you want Twitter to redirect to after authorization</param>
        public new void BeginAuthorization(Uri callback, bool forceLogin)
        {
            base.BeginAuthorization(callback, forceLogin);
        }
    }
}
