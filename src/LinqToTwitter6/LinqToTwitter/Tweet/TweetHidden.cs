using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetHidden
    {
        [JsonPropertyName("hidden")]
        public bool Hidden { get; init; }
    }
}