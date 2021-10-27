using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record DeleteListRequest
    {
        [JsonPropertyName("id")]
        public string? ID { get; set; }
    }
}