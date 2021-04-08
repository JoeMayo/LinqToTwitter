using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record BlockingResponseData
    {
        [JsonPropertyName("blocking")]
        public bool Blocking { get; set; }
    }
}
