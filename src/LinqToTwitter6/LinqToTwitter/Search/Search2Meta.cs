using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metadata for <see cref="Search2"/> query results
    /// </summary>
    public class Search2Meta
    {
        /// <summary>
        /// Most recent ID in the data
        /// </summary>
        [JsonPropertyName("newest_id")]
        public string? NewestID { get; set; }

        /// <summary>
        /// Oldest ID in the data
        /// </summary>
        [JsonPropertyName("oldest_id")]
        public string? OldestID { get; set; }

        /// <summary>
        /// Number of results
        /// </summary>
        [JsonPropertyName("result_count")]
        public int Count { get; set; }
    }
}
