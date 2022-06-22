using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class BookmarkedTweetID
    {
        [JsonPropertyName("tweet_id")]
        public string TweetID { get; set; } = string.Empty;
    }
}
