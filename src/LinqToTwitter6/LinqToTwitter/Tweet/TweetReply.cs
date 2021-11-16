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
        public string? InReplyToTweetID { get; set; }

        /// <summary>
        /// IDs of users to remove from conversation
        /// </summary>
        [JsonPropertyName("exclude_reply_user_ids")]
        public List<string>? ExcludeReplyUserIds { get; set; }
    }
}
