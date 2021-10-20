using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record SpaceResponseData
    {
        [JsonPropertyName("muting")]
        public bool Muting { get; set; }
    }
}
