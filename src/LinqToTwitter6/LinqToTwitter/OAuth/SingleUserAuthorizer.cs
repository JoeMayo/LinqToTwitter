using LinqToTwitter.OAuth;
using System;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class SingleUserAuthorizer : AuthorizerBase, IAuthorizer
    {
        /// <summary>
        /// Not required. Since you've already provided all 4 tokens, you don't need to call this.
        /// </summary>
        public async Task AuthorizeAsync()
        {
            if (!CredentialStore.HasAllCredentials())
                throw new InvalidOperationException("SingleUserAuthorizer needs pre-set credentials; don't call Authorize unless you've set all four credentials.");

            await Task.Delay(0).ConfigureAwait(false);

            return;
        }
    }
}
