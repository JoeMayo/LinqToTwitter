using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace LinqToTwitter
{
    public class UniversalAuthorizer : AuthorizerBase, IAuthorizer
    {
        const string CallbackPlaceholderRequiredByTwitterOauth = "http://localhost.com";

        public async Task AuthorizeAsync()
        {
            if (CredentialStore == null)
                throw new NullReferenceException(
                    "The authorization process requires a minimum of ConsumerKey and ConsumerSecret tokens. " +
                    "You must assign the CredentialStore property (with tokens) before calling AuthorizeAsync().");

            if (CredentialStore.HasAllCredentials()) return;

            if (string.IsNullOrWhiteSpace(CredentialStore.ConsumerKey) || string.IsNullOrWhiteSpace(CredentialStore.ConsumerSecret))
                throw new ArgumentException("You must populate CredentialStore with ConsumerKey and ConsumerSecret tokens before calling AuthorizeAsync.", "CredentialStore");

            string authCallback = 
                string.IsNullOrWhiteSpace(Callback) ? CallbackPlaceholderRequiredByTwitterOauth : Callback;
            await GetRequestTokenAsync(authCallback);

            string authUrl = PrepareAuthorizeUrl(ForceLogin);

            WebAuthenticationResult webAuthenticationResult =
                await WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None,
                    new Uri(authUrl),
                    new Uri(authCallback));

            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                string verifier = ParseVerifierFromResponseUrl(webAuthenticationResult.ResponseData);
                var accessTokenParams = new Dictionary<string, string>();
                accessTokenParams.Add("oauth_verifier", verifier);

                await GetAccessTokenAsync(accessTokenParams);
            }
        }
    }
}
