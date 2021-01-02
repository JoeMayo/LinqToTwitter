using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter.Common
{
    public record TwitterGeo
    {
        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("bbox")]
        public List<float>? BBox { get; init; }

        [JsonPropertyName("properties")]
        public TwitterProperties? Properties { get; init; }
    }
}
