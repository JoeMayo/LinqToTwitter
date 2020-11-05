using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record DeleteStreamingFilterRules
    {
        [JsonPropertyName("delete")]
        public DeleteIds? Delete { get; set; }
    }
}
