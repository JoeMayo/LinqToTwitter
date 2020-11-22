using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Entities associated with a user profile description
    /// </summary>
    public record TwitterUserDescriptionEntities
    {
        /// <summary>
        /// Urls in the profile description
        /// </summary>
        [JsonPropertyName("urls")]
        public List<TweetEntityUrl>? Urls { get; init; }

        /// <summary>
        /// Hashtags in the profile description
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<TweetEntityHashtag>? Hashtags { get; init; }

        /// <summary>
        /// Mentions in the profile description
        /// </summary>
        [JsonPropertyName("mentions")]
        public List<TweetEntityMention>? Mentions { get; init; }
    }
}
