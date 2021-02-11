using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TwitterUserFollowResponse
    {
        [JsonPropertyName("data")]
        public TwitterUserFollowResponseData? Data { get; init; }
    }
}