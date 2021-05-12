using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class LikedTweetID
    {
        [JsonPropertyName("tweet_id")]
        public string TweetID { get; set; } = string.Empty;
    }
}
