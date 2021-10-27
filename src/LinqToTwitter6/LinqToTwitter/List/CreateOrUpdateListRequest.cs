using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record CreateOrUpdateListRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("private")]
        public bool Private { get; set; }
    }
}