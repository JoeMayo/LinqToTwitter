using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter.Common
{
    public record TwitterInclude
    {
        [JsonPropertyName("users")]
        public List<TwitterUser>? Users { get; init; }

        [JsonPropertyName("tweets")]
        public List<Tweet>? Tweets { get; init; }

        [JsonPropertyName("places")]
        public List<TwitterPlace>? Places { get; init; }

        [JsonPropertyName("polls")]
        public List<TwitterPoll>? Polls { get; init; }

        [JsonPropertyName("media")]
        public List<TwitterMedia>? Media { get; init; }
    }
}
