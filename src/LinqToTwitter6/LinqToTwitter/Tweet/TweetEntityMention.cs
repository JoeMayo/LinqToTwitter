using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a mention entity
    /// </summary>
    public record TweetEntityMention
    {
        /// <summary>
        /// Start of mention
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; init; }

        /// <summary>
        /// End of mention
        /// </summary>
        [JsonPropertyName("end")]
        public int End { get; init; }

        /// <summary>
        /// Mentioned username
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; init; }
    }
}
