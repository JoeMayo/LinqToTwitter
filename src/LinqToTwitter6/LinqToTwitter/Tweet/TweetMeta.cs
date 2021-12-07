using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metadata for tweet queries
    /// </summary>
    public class TweetMeta
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
