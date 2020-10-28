using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter.Common
{
    public record TwitterGeo
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("bbox")]
        public List<float>? BBox { get; set; }

        [JsonPropertyName("properties")]
        public TwitterProperties? Properties { get; set; }
    }
}
