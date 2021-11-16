using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// For tweeting uploaded media
    /// </summary>
    public record TweetMedia
    {
        /// <summary>
        /// IDs of uploaded media
        /// </summary>
        [JsonPropertyName("media_ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? MediaIds { get; set; }

        /// <summary>
        /// IDs of tagged users
        /// </summary>
        [JsonPropertyName("tagged_user_ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? TaggedUserIds { get; set; }
    }
}
