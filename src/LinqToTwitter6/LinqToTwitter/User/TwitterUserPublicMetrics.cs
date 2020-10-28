using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Public metrics for a user
    /// </summary>
    public record TwitterUserPublicMetrics
    {
        /// <summary>
        /// Number of people following the user
        /// </summary>
        [JsonPropertyName("followers_count")]
        public int FollowersCount { get; set; }

        /// <summary>
        /// Number of people user is following
        /// </summary>
        [JsonPropertyName("following_count")]
        public int FollowingCount { get; set; }

        /// <summary>
        /// Number of times user tweeted
        /// </summary>
        [JsonPropertyName("tweet_count")]
        public int TweetCount { get; set; }

        /// <summary>
        /// Number of lists others have added this user to
        /// </summary>
        [JsonPropertyName("listed_count")]
        public int ListedCount { get; set; }
    }
}
