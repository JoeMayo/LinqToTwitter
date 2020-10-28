using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Errors returned from a Twitter Search
    /// </summary>
    public record TwitterSearchError
    {
        [JsonPropertyName("resource_type")]
        public string? ResourceType { get; set; }

        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("section")]
        public string? Section { get; set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

