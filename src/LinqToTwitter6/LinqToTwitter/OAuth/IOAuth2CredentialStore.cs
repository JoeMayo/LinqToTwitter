using System.Collections.Generic;

namespace LinqToTwitter.OAuth
{
    public interface IOAuth2CredentialStore : ICredentialStore
    {
        /// <summary>
        /// Required - you can find this in the Twitter Developer portal
        /// </summary>
        string? ClientID { get; set; }

        /// <summary>
        /// Required for confidential clients - you can find this in the Twitter Developer portal
        /// </summary>
        string? ClientSecret { get; set; }

        /// <summary>
        /// Required - these are the permissions you want the user to give your app.
        /// See endpoint documentation for what scopes are required.
        /// </summary>
        IEnumerable<string>? Scopes { get; set; }

        /// <summary>
        /// Populated after user approves your app and used for each command/query
        /// </summary>
        string? AccessToken { get; set; }

        /// <summary>
        /// Can be used to get a new AccessToken - only available if you authorized with `offline.access` scope.
        /// </summary>
        string? RefreshToken { get; set; }

        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        string? RedirectUri { get; set; }

        /// <summary>
        /// Send when authorizing and getting access token to verify original source
        /// </summary>
        string? CodeChallenge { get; set; }

        /// <summary>
        /// Value to verify against what was sent to Twitter and what was received.
        /// Helps prevent CSRF attack.
        /// </summary>
        string? State { get; set; }
    }
}
