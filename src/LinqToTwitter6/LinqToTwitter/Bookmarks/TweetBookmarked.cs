using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetBookmarked
    {
        [JsonPropertyName("bookmarked")]
        public bool Bookmarked { get; init; }
    }
}