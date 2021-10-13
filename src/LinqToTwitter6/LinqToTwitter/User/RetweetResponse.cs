using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record RetweetResponse
    {
        [JsonPropertyName("data")]
        public RetweetResponseData? Data { get; init; }
    }
}