using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Rule on filter stream
    /// </summary>
    public record StreamingRule
    {
        /// <summary>
        /// ID for rule
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Rule text
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Rule tag
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }
    }

}
