using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Tweet image details
    /// </summary>
    public record TweetEntityImage
    {
        /// <summary>
        /// Image URL
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; }

        /// <summary>
        /// Image width
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; init; }

        /// <summary>
        /// Image height
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; init; }
    }
}
