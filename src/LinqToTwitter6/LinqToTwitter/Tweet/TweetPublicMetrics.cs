using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metrics available to the public
    /// </summary>
    public record TweetPublicMetrics
    {
        /// <summary>
        /// Number of retweets
        /// </summary>
        [JsonPropertyName("retweet_count")]
        public int RetweetCount { get; init; }

        /// <summary>
        /// Number of replies
        /// </summary>
        [JsonPropertyName("reply_count")]
        public int ReplyCount { get; init; }

        /// <summary>
        /// Number of Likes
        /// </summary>
        [JsonPropertyName("like_count")]
        public int LikeCount { get; init; }

        /// <summary>
        /// Number of quotes
        /// </summary>
        [JsonPropertyName("quote_count")]
        public int QuoteCount { get; init; }

        /// <summary>
        /// Number of bookmarks
        /// </summary>
        [JsonPropertyName("bookmark_count")]
        public int BookmarkCount { get; init; }

        /// <summary>
        /// Number of Views
        /// </summary>
        [JsonPropertyName("impression_count")]
        public int ImpressionCount { get; init; }
    }
}
