using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    /// <summary>
    /// Manages credentials for OAuth2 Authentications
    /// </summary>
    public class OAuth2CredentialStore : InMemoryCredentialStore, IOAuth2CredentialStore
    {
        /// <summary>
        /// Required for all clients - you can find this in the Twitter Developer portal
        /// </summary>
        public virtual string? ClientID { get; set; }

        /// <summary>
        /// Required for confidential clients - you can find this in the Twitter Developer portal
        /// </summary>
        public virtual string? ClientSecret { get; set; }

        /// <summary>
        /// Send when authorizing and getting access token to verify original source
        /// </summary>
        public virtual string? CodeChallenge { get; set; }

        /// <summary>
        /// Required - these are the permissions you want the user to give your app.
        /// See endpoint documentation for what scopes are required.
        /// </summary>
        public virtual IEnumerable<string>? Scopes { get; set; }

        /// <summary>
        /// Populated after user approves your app and used for each command/query
        /// </summary>
        public virtual string? AccessToken { get; set; }

        /// <summary>
        /// Can be used to get a new AccessToken - only available if you authorized with `offline.access` scope.
        /// </summary>
        public virtual string? RefreshToken { get; set; }

        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public virtual string? RedirectUri { get; set; }

        /// <summary>
        /// Value to verify against what was sent to Twitter and what was received.
        /// Helps prevent CSRF attack.
        /// </summary>
        public virtual string? State { get; set; }

        /// <summary>
        /// Sets all properties to default values
        /// </summary>
        public override async Task ClearAsync()
        {
            await base.ClearAsync();

            AccessToken = default;
            ClientID = default;
            ClientSecret = default;
            CodeChallenge = default;
            RefreshToken = default;
            RedirectUri = default;
            Scopes = default;
            State = default;
        }

        public override bool HasAllCredentials() => !string.IsNullOrWhiteSpace(AccessToken);
    }
}
