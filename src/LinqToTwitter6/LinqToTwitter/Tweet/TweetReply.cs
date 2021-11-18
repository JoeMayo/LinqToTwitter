using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Request data for replying to a tweet
    /// </summary>
    public record TweetReply
    {
        /// <summary>
        /// ID of tweet being replied to
        /// </summary>
        [JsonPropertyName("in_reply_to_tweet_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InReplyToTweetID { get; set; }

        /// <summary>
        /// IDs of users to remove from conversation
        /// </summary>
        [JsonPropertyName("exclude_reply_user_ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? ExcludeReplyUserIds { get; set; }
    }
}
