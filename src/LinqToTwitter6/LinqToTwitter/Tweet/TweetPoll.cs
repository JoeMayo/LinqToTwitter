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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Options { get; init; }

        /// <summary>
        /// Number of minutes to run poll
        /// </summary>
        [JsonPropertyName("duration_minutes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int DurationMinutes { get; init; }
    }
}
