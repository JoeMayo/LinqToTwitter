using System;

namespace LinqToTwitter
{
    public class SingleUserAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Required by interface to integrate with TwitterContext, but don't call (not used).
        /// </summary>
        public void Authorize()
        {
            if (!IsAuthorized)
                throw new InvalidOperationException("SingleUserAuthorizer needs preset credentials; don't call Authorize unless you've completly set the Credentials");
        }
    }
}
