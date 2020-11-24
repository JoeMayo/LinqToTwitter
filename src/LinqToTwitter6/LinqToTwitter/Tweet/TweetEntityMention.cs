using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a mention entity
    /// </summary>
    public record TweetEntityMention
    {
        /// <summary>
        /// Start of hashtag
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; init; }

        /// <summary>
        /// End of hashtag
        /// </summary>
        [JsonPropertyName("end")]
        public int End { get; init; }

        /// <summary>
        /// Hashtag text
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; init; }
    }
}
