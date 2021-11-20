using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetDeleted
    {
        [JsonPropertyName("deleted")]
        public bool Deleted { get; init; }
    }
}