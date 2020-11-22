using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TwitterUserUrlEntities
    {
        /// <summary>
        /// Collection of URLs attached to Tweet
        /// </summary>
        [JsonPropertyName("urls")]
        public List<TweetEntityUrl>? Urls { get; init; }
    }
}
