using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// New rule on filter stream
    /// </summary>
    public record StreamingAddRule
    {
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
