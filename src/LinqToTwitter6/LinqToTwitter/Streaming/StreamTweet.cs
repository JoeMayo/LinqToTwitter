using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record StreamTweet
    {
        [JsonPropertyName("data")]
        public Tweet? Tweet { get; init; }

        [JsonPropertyName("matching_rules")]
        public List<MatchingRule>? MatchingRules { get; init; }
    }
}
