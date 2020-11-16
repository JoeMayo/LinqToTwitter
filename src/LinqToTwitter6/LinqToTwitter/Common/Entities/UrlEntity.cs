using System.Text.Json.Serialization;

namespace LinqToTwitter.Common.Entities
{
    /// <summary>
    /// Url mention in the tweet
    /// </summary>
    /// <example>http://bit.ly/129Ad</example>
    public class UrlEntity : EntityBase
    {
        /// <summary>
        /// Absolute Url in the tweet
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// t.co shortened URL
        /// </summary>
        [JsonPropertyName("display_url")]
        public string? DisplayUrl { get; set; }

        /// <summary>
        /// t.co expanded URL
        /// </summary>
        [JsonPropertyName("expanded_url")]
        public string? ExpandedUrl { get; set; }

        /// <summary>
        /// Locations for begin/end index of where URL occurs.
        /// </summary>
        [JsonPropertyName("indices")]
        public int[]? Indices { get; set; }
    }
}
