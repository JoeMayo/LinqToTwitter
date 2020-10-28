using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// References a tweet
    /// </summary>
    public record TweetReference
    {
        /// <summary>
        /// What kind of reference. e.g. reply, quote, retweet etc.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// ID of referenced tweet
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }
    }
}
