using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metadata for <see cref="TwitterSearch"/> query results
    /// </summary>
    public record TwitterSearchMeta
    {
        /// <summary>
        /// Most recent ID in the data
        /// </summary>
        [JsonPropertyName("newest_id")]
        public string? NewestID { get; init; }

        /// <summary>
        /// Oldest ID in the data
        /// </summary>
        [JsonPropertyName("oldest_id")]
        public string? OldestID { get; init; }

        /// <summary>
        /// Number of results
        /// </summary>
        [JsonPropertyName("result_count")]
        public int Count { get; init; }
    }
}
