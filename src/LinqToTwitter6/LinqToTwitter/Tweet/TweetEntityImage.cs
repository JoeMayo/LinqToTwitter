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
        public string? Url { get; set; }

        /// <summary>
        /// Image width
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// Image height
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}
