using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a hashtag entity
    /// </summary>
    public record TweetEntityHashtag
    {
        /// <summary>
        /// Start of hashtag
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; set; }

        /// <summary>
        /// End of hashtag
        /// </summary>
        [JsonPropertyName("end")]
        public int End { get; set; }

        /// <summary>
        /// Hashtag text
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }
    }
}
