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
        public List<TweetEntityUrl>? Urls { get; set; }
        
        /// <summary>
        /// Hashtag entities
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<TweetEntityHashtag>? Hashtags { get; set; }

        /// <summary>
        /// Mention entities
        /// </summary>
        [JsonPropertyName("mentions")]
        public List<TweetEntityMention>? Mentions { get; set; }

        /// <summary>
        /// Annotation entities
        /// </summary>
        [JsonPropertyName("annotations")]
        public List<TweetEntityAnnotation>? Annotations { get; set; }
    }
}
