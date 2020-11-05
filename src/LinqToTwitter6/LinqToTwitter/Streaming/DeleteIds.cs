using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record DeleteIds
    {
        [JsonPropertyName("ids")]
        public List<string>? Ids { get; set; }
    }
}
