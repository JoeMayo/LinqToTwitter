using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record RetweetTweetID
    {
        [JsonPropertyName("tweet_id")]
        public string? TweetID { get; init; }
    }
}