using LinqToTwitter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetQuery
    {
        //
        // Query input fields
        //

        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        public TweetType Type { get; init; }

        /// <summary>
        /// UTC date/time to search to
        /// </summary>
        public DateTime EndTime { get; init; }

        /// <summary>
        /// Comma-separated list of tweet types to exclude
        /// </summary>
        public string? Exclude { get; init; }

        /// <summary>
        /// Comma-separated list of expansion fields
        /// </summary>
        public string? Expansions { get; init; }

        /// <summary>
        /// Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; init; }

        /// <summary>
        /// User ID for timeline queries
        /// </summary>
        public string? ID { get; init; }

        /// <summary>
        /// ID for list to get tweets from
        /// </summary>
        public string? ListID { get; init; }

        /// <summary>
        /// Max number of tweets to return per requrest - default 10 - possible 100
        /// </summary>
        public int MaxResults { get; init; }

        /// <summary>
        /// Comma-separated list of fields to return in the media object
        /// </summary>
        public string? MediaFields { get; init; }

        /// <summary>
        /// If set, with token from previous response metadata, pages forward or backward
        /// </summary>
        public string? PaginationToken { get; init; }

        /// <summary>
        /// Comma-separated list of fields to return in the place object
        /// </summary>
        public string? PlaceFields { get; init; }

        /// <summary>
        /// Comma-separated list of fields to return in the poll object
        /// </summary>
        public string? PollFields { get; init; }

        /// <summary>
        /// returns tweets later than this ID
        /// </summary>
        public string? SinceID { get; init; }

        /// <summary>
        /// Date to search from
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// ID of space to query for tweets
        /// </summary>
        public string? SpaceID { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object
        /// </summary>
        public string? TweetFields { get; init; }

        /// <summary>
        /// returns tweets earlier than this ID
        /// </summary>
        public string? UntilID { get; init; }

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
        public List<Tweet>? Tweets { get; init; }

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

        /// <summary>
        /// Metadata with count and paging details
        /// </summary>
        [JsonPropertyName("meta")]
        public TweetMeta? Meta { get; set; }
    }
}
