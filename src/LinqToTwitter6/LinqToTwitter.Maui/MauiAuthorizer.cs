using LinqToTwitter.OAuth;

namespace LinqToTwitter.Maui
{
    public class MauiAuthorizer : OAuth2Authorizer
    {
        public new async Task AuthorizeAsync()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            credStore.State = GenerateCodeChallenge();

            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.RedirectUri))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.RedirectUri)} before calling AuthorizeAsync.", nameof(credStore.RedirectUri));
            if (!credStore.Scopes?.Any() ?? false)
                throw new ArgumentException($"You must populate CredentialStore with {nameof(credStore.Scopes)} (permissions) before calling AuthorizeAsync.", nameof(credStore.Scopes));

            string authUrl = PrepareAuthorizeUrl(credStore.State);

            var authResult =
                await WebAuthenticator.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(credStore.RedirectUri));

            await CompleteAuthorizeAsync(authResult.Properties["code"], authResult.Properties["state"]);

            await Task.CompletedTask;
        }
    }
}