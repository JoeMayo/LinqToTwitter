using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metadata for list queries
    /// </summary>
    public class ListMeta
    {
        /// <summary>
        /// Number of results returned
        /// </summary>
        [JsonPropertyName("result_count")] 
        public int ResultCount { get; set; }

        /// <summary>
        /// Token for previous page results
        /// </summary>
        [JsonPropertyName("previous_token")]
        public string? PreviousToken { get; set; }

        /// <summary>
        /// Token for next page events
        /// </summary>
        [JsonPropertyName("next_token")]
        public string? NextToken { get; set; }
    }
}
