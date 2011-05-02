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
            throw new InvalidOperationException("SingleUserAuthorizer only needs credentials; don't call Authorize - just pass instance to TwitterContext");
        }
    }
}
