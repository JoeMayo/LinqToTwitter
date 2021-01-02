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

        [JsonPropertyName("media")]
        public List<TweetMedia>? Media { get; init; }
    }
}
