using System.Text.Json.Serialization;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// Represents a Media object, such as gif, photo, or video
    /// </summary>
    public record TwitterMedia
    {
        /// <summary>
        /// Alt text to display with an image
        /// </summary>
        [JsonPropertyName("alt_text")]
        public string? AltText { get; set; }

        /// <summary>
        /// Milliseconds duration for videos
        /// </summary>
        [JsonPropertyName("duration_ms")]
        public int DurationMS { get; init; }

        /// <summary>
        /// Height in pixels
        [JsonPropertyName("height")]
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Media ID - Matches MediaKey in TweetAttachments
        /// </summary>
        [JsonPropertyName("media_key")]
        public string? MediaKey { get; init; }

        // TODO
        [JsonPropertyName("non_public_metrics")]
        public object? NonPublicMetrics { get; init; }

        // TODO
        [JsonPropertyName("organic_metrics")]
        public object? OrganicMetrics { get; init; }

        /// <summary>
        /// URL to animated GIF and video preview image
        /// </summary>
        [JsonPropertyName("preview_image_url")]
        public string? PreviewImageUrl { get; init; }

        // TODO
        [JsonPropertyName("promoted_metrics")]
        public object? PromotedMetrics { get; init; }

        // TODO
        [JsonPropertyName("public_metrics")]
        public object? PublicMetrics { get; init; }

        /// <summary>
        /// Type of media - e.g. gif, photo, or video
        /// </summary>
        [JsonConverter(typeof(TweetMediaTypeConverter))]
        [JsonPropertyName("type")]
        public TweetMediaType Type { get; init; }

        /// <summary>
        /// URL to photo preview image
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; }

        /// <summary>
        /// Width in pixels
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; init; }
    }
}
