using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents an annotation entity
    /// </summary>
    public record TweetEntityAnnotation
    {
        /// <summary>
        /// Start of annotation text
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; set; }

        /// <summary>
        /// End of annotation text
        /// </summary>
        [JsonPropertyName("end")]
        public int End { get; set; }

        /// <summary>
        /// Probability that <see cref="NormalizedText"/> belongs to <see cref="Type"/> category
        /// </summary>
        [JsonPropertyName("probability")]
        public float Probability { get; set; }

        /// <summary>
        /// Category that <see cref="NormalizedText"/> belongs to
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Annotated text
        /// </summary>
        [JsonPropertyName("normalized_text")]
        public string? NormalizedText { get; set; }
    }
}
