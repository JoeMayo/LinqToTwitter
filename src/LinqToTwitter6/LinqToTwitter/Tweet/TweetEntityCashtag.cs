using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a hashtag entity
    /// </summary>
    public record TweetEntityCashtag
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
        [JsonPropertyName("tag")]
        public string? Tag { get; init; }
    }
}
