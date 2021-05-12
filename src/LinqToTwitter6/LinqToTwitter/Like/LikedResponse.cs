using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record LikedResponse
    {
        [JsonPropertyName("data")]
        public TweetLiked? Data { get; init; }
    }
}