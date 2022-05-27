using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Metadata for tweet timeline queries
    /// </summary>
    public class TweetMeta
    {
        /// <summary>
        /// First ID in the list
        /// </summary>
        [JsonPropertyName("newest_id")]
        public string? NewestID { get; set; }

        /// <summary>
        /// Token for next page events
        /// </summary>
        [JsonPropertyName("next_token")]
        public string? NextToken { get; set; }

        /// <summary>
        /// Last ID in the list
        /// </summary>
        [JsonPropertyName("oldest_id")]
        public string? OldestID { get; set; }

        /// <summary>
        /// Token for previous page results
        /// </summary>
        [JsonPropertyName("previous_token")]
        public string? PreviousToken { get; set; }

        /// <summary>
        /// Number of results returned
        /// </summary>
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }
    }
}
