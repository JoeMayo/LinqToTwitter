using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Tweet context annotation - a Domain/Entity pair
    /// </summary>
    public record TweetContextAnnotation
    {
        /// <summary>
        /// Domain details
        /// </summary>
        [JsonPropertyName("domain")]
        public TweetContextAnnotationDetails? Domain { get; set; }
        
        /// <summary>
        /// Entity details
        /// </summary>
        [JsonPropertyName("entity")]
        public TweetContextAnnotationDetails? Entity { get; set; }
    }
}
