using LinqToTwitter.OAuth;

namespace LinqToTwitter
{
    /// <summary>
    /// Implements the "Sign-in With Twitter" feature
    /// </summary>
    public class MvcSignInAuthorizer : MvcAuthorizer, IAuthorizer
    {
        public MvcSignInAuthorizer()
        {
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authenticate";
        }
    }
}
