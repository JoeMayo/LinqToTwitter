using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public enum TweetReplySettings
    {
        /// <summary>
        /// Not set
        /// </summary>
        [JsonPropertyName("")]
        None,

        /// <summary>
        /// Anyone can reply to this tweet
        /// </summary>
        [JsonPropertyName("everyone")]
        Everyone,

        /// <summary>
        /// Only the users mentioned in the tweet
        /// </summary>
        [JsonPropertyName("mentionedUsers")]
        MentionedUsers,

        /// <summary>
        /// Anyone following the user who tweeted
        /// </summary>
        [JsonPropertyName("following")]
        Following
    }
}
