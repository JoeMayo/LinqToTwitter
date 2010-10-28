using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Implements the "Sign-in With Twitter" feature
    /// </summary>
    [Serializable]
    public class SignInAuthorizer : WebAuthorizer, ITwitterAuthorizer
    {
        public SignInAuthorizer()
        {
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authenticate";
        }


    }
}
