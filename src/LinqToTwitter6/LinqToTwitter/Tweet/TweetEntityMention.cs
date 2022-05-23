using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a mention entity
    /// </summary>
    public record TweetEntityMention
    {
        /// <summary>
        /// End of mention
        /// </summary>
        [JsonPropertyName("end")]
        public int End { get; init; }

        /// <summary>
        /// ID of mentioned user
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Start of mention
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; init; }

        /// <summary>
        /// Mentioned username
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; init; }
    }
}
