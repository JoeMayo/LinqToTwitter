using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a Twitter poll
    /// </summary>
    public record TweetPoll
    {
        /// <summary>
        /// ID for the poll
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; init; }

        /// <summary>
        /// Poll options
        /// </summary>
        [JsonPropertyName("options")]
        public List<TweetPollOption>? Options { get; init; }

        /// <summary>
        /// Number of minutes to run poll
        /// </summary>
        [JsonPropertyName("duration_minutes")]
        public int DurationMinutes { get; init; }

        /// <summary>
        /// Ending date and time
        /// </summary>
        [JsonPropertyName("end_datetime")]
        public DateTime EndDatetime { get; init; }

        /// <summary>
        /// Whether poll is open or closed
        /// </summary>
        [JsonPropertyName("voting_status")]
        public string? VotingStatus { get; init; }
    }
}
