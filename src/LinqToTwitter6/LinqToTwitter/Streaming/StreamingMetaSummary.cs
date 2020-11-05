using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from either adding or deleting rules
    /// </summary>
    public record StreamingMetaSummary
    {
        /// <summary>
        /// Number of rules created
        /// </summary>
        [JsonPropertyName("created")]
        public int Created { get; set; }

        /// <summary>
        /// Number of rules not created
        /// </summary>
        [JsonPropertyName("not_created")]
        public int NotCreated { get; set; }

        /// <summary>
        /// Number of rules deleted
        /// </summary>
        [JsonPropertyName("deleted")]
        public int Deleted { get; set; }

        /// <summary>
        /// Number of rules not deleted
        /// </summary>
        [JsonPropertyName("not_deleted")]
        public int NotDeleted { get; set; }
    }
}
