using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Entities associated with a user
    /// </summary>
    public record TwitterUserEntity
    {
        /// <summary>
        /// Entities from user profile URL
        /// </summary>
        [JsonPropertyName("url")]
        public TwitterUserUrlEntities? Url { get; init; }

        /// <summary>
        /// Entities from user profile description
        /// </summary>
        [JsonPropertyName("description")]
        public TwitterUserDescriptionEntities? Description { get; init; }
    }
}
