using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class XAuthAuthorizer : AuthorizerBase, IAuthorizer
    {
        public async Task AuthorizeAsync()
        {
            var xauthCredentials = CredentialStore as XAuthCredentials;
            var postData = new Dictionary<string, string>
            {
                {"x_auth_username", xauthCredentials.UserName},
                {"x_auth_password", xauthCredentials.Password},
                {"x_auth_mode", "client_auth"}
            };

            await PostAccessTokenAsync(postData).ConfigureAwait(false);
        }
    }
}
