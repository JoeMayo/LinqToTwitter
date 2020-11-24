using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Entities associated with a tweet
    /// </summary>
    public record TweetEntities
    {
        /// <summary>
        /// URL entities
        /// </summary>
        [JsonPropertyName("urls")]
        public List<TweetEntityUrl>? Urls { get; init; }
        
        /// <summary>
        /// Hashtag entities
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<TweetEntityHashtag>? Hashtags { get; init; }

        /// <summary>
        /// Mention entities
        /// </summary>
        [JsonPropertyName("mentions")]
        public List<TweetEntityMention>? Mentions { get; init; }

        /// <summary>
        /// Annotation entities
        /// </summary>
        [JsonPropertyName("annotations")]
        public List<TweetEntityAnnotation>? Annotations { get; init; }
    }
}
