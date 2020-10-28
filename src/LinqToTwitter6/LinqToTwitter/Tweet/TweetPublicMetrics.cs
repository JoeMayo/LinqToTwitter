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
        public int RetweetCount { get; set; }

        /// <summary>
        /// Number of replies
        /// </summary>
        [JsonPropertyName("reply_count")]
        public int ReplyCount { get; set; }

        /// <summary>
        /// Number of Likes
        /// </summary>
        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        /// <summary>
        /// Number of quotes
        /// </summary>
        [JsonPropertyName("quote_count")]
        public int QuoteCount { get; set; }
    }
}
