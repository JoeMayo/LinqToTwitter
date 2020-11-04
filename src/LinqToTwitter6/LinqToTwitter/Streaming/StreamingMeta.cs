using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record StreamingMeta
    {
        [JsonPropertyName("sent")]
        public DateTime Sent { get; set; }
    }
}
