using System;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class SingleUserAuthorizer : AuthorizerBase, IAuthorizer
    {
        /// <summary>
        /// Not required. Since you've already provided all 4 tokens, you don't need to call this.
        /// </summary>
        public async Task AuthorizeAsync()
        {
            if (!CredentialStore.HasAllCredentials())
                throw new InvalidOperationException("SingleUserAuthorizer needs preset credentials; don't call Authorize unless you've set all four credentials.");
        }
    }
}
