using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Rule on filter stream
    /// </summary>
    public record MatchingRule
    {
        /// <summary>
        /// ID for rule
        /// </summary>
        [JsonPropertyName("id")]
        public ulong ID { get; set; }

        /// <summary>
        /// Rule tag
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }
    }

}
