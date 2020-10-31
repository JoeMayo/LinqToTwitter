using System.Text.Json.Serialization;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// Errors returned from a Twitter Queries.
    /// </summary>
    /// <remarks>
    /// Meaning of each property depends on type of query. Some will be null.
    /// </remarks>
    public record TwitterError
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

        [JsonPropertyName("parameter")]
        public string? Parameter { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

