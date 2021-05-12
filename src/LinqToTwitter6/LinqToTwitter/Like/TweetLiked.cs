using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetLiked
    {
        [JsonPropertyName("liked")]
        public bool Liked { get; init; }
    }
}