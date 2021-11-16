using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Request data for Creating a Poll
    /// </summary>
    public record TweetPoll
    {
        /// <summary>
        /// Poll options
        /// </summary>
        [JsonPropertyName("options")]
        public List<string>? Options { get; init; }

        /// <summary>
        /// Number of minutes to run poll
        /// </summary>
        [JsonPropertyName("duration_minutes")]
        public int DurationMinutes { get; init; }
    }
}
