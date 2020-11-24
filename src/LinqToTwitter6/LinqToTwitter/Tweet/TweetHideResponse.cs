using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetHideResponse
    {
        [JsonPropertyName("data")]
        public TweetHidden? Data { get; init; }
    }
}