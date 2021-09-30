using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record MuteResponseData
    {
        [JsonPropertyName("muting")]
        public bool Muting { get; set; }
    }
}
