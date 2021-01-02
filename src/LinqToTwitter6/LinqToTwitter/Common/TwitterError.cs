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
        public string? ResourceType { get; init; }

        [JsonPropertyName("field")]
        public string? Field { get; init; }

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("section")]
        public string? Section { get; init; }

        [JsonPropertyName("detail")]
        public string? Detail { get; init; }

        [JsonPropertyName("id")]
        public string? ID { get; init; }

        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("parameter")]
        public string? Parameter { get; init; }

        [JsonPropertyName("value")]
        public string? Value { get; init; }
    }
}

