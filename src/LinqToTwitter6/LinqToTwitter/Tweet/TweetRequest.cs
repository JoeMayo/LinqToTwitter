using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Request parameters for posting a tweet
    /// </summary>
    public record TweetRequest
    {
        /// <summary>
        /// Deep link to a direct message, e.g. https://twitter.com/messages/compose?recipient_id=2244994945
        /// </summary>
        [JsonPropertyName("direct_message_deep_link")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DirectMessageDeepLink { get; set; }

        /// <summary>
        /// Only super followers can see
        /// </summary>
        [JsonPropertyName("for_super_followers_only")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool ForSuperFollowersOnly { get; set; }

        /// <summary>
        /// Geographical location of tweet
        /// </summary>
        [JsonPropertyName("geo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TweetGeo? Geo { get; set; }

        /// <summary>
        /// For tweeting uploaded media
        /// </summary>
        [JsonPropertyName("media")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TweetMedia? Media { get; set; }

        /// <summary>
        /// Create a new poll
        /// </summary>
        [JsonPropertyName("poll")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TweetPoll? Poll { get; set; }

        /// <summary>
        /// ID of tweet being quoted/retweeted
        /// </summary>
        [JsonPropertyName("quote_tweet_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? QuoteTweetID { get; set; }

        /// <summary>
        /// Info for replying to a tweet
        /// </summary>
        [JsonPropertyName("reply")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TweetReply? Reply { get; set; }

        /// <summary>
        /// Specify who can reply to a tweet
        /// </summary>
        [JsonPropertyName("reply_settings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TweetReplySettings? ReplySettings { get; set; }

        /// <summary>
        /// Tweet text
        /// </summary>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }
    }
}
