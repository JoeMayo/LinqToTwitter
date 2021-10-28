using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record ListDeleteRequest
    {
        [JsonPropertyName("id")]
        public string? ID { get; set; }
    }
}