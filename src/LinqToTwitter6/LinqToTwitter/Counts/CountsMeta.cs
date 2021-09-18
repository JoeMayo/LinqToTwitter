using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record CountsMeta
    {
        [JsonPropertyName("total_tweet_count")]
        public int TotalTweetCount { get; set; }
    }
}
