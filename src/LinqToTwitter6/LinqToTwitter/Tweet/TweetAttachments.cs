using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Tweet attachments
    /// </summary>
    public record TweetAttachments
    {
        /// <summary>
        /// Poll IDs
        /// </summary>
        [JsonPropertyName("poll_ids")]
        public List<string>? PollIds { get; set; }

        /// <summary>
        /// Media Keys
        /// </summary>
        [JsonPropertyName("media_keys")]
        public List<string>? MediaKeys { get; set; }
    }
}
