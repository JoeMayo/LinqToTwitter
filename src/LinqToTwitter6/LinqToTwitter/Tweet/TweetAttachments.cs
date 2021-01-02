using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Type of attachment in a tweet
    /// </summary>
    public record TweetAttachments
    {
        /// <summary>
        /// Poll IDs
        /// </summary>
        [JsonPropertyName("poll_ids")]
        public List<string>? PollIds { get; init; }

        /// <summary>
        /// Media Keys
        /// </summary>
        [JsonPropertyName("media_keys")]
        public List<string>? MediaKeys { get; init; }
    }
}
