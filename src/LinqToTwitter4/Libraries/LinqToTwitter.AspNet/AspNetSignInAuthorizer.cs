using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Implements the "Sign-in With Twitter" feature
    /// </summary>
    public class AspNetSignInAuthorizer : AspNetAuthorizer, IAuthorizer
    {
        public AspNetSignInAuthorizer()
        {
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authenticate";
        }
    }
}
