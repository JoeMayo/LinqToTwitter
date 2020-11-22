using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TwitterUserQuery
    {
        //
        // Query input fields
        //

        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        public UserType Type { get; init; }

        /// <summary>
        /// Required for id queries - Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; init; }

        /// <summary>
        /// Required for username queries - Up to 100 comma-separated usernames to search for
        /// </summary>
        public string? Usernames { get; init; }

        /// <summary>
        /// Comma-separated list of expansion fields
        /// </summary>
        public string? Expansions { get; init; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object
        /// </summary>
        public string? TweetFields { get; init; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object
        /// </summary>
        public string? UserFields { get; init; }

        //
        // Output results
        //

        /// <summary>
        /// Tweet data returned from the search
        /// </summary>
        [JsonPropertyName("data")]
        public List<TwitterUser>? Users { get; init; }

        /// <summary>
        /// If any errors occur, they'll show up here
        /// </summary>
        [JsonPropertyName("errors")]
        public List<TwitterError>? Errors { get; init; }

        /// <summary>
        /// Were there errors?
        /// </summary>
        public bool HasErrors { get => Errors?.Any() ?? false; }

        /// <summary>
        /// Populated when query includes expansion fields
        /// </summary>
        [JsonPropertyName("includes")]
        public TwitterInclude? Includes { get; init; }
    }
}
