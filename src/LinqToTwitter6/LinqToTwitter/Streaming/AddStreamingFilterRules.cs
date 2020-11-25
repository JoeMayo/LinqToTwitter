using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record AddStreamingFilterRules
    {
        [JsonPropertyName("add")]
        public List<StreamingAddRule>? Add { get; init; }
    }
}
