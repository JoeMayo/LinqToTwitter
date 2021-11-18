using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetDeletedResponse
    {
        [JsonPropertyName("data")]
        public TweetDeleted? Data { get; init; }
    }
}