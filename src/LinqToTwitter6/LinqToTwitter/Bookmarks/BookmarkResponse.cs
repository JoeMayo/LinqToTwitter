using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record BookmarkResponse
    {
        [JsonPropertyName("data")]
        public TweetBookmarked? Data { get; init; }
    }
}