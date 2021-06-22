using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record BlocksMeta
    {
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }
    }
}