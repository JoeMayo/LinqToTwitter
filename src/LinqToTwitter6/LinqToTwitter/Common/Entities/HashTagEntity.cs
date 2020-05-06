using System.Text.Json.Serialization;

namespace LinqToTwitter.Common.Entities
{
    /// <summary>
    /// Hash tag entity
    /// </summary>
    /// <example>#linqtotwitter</example>
    public class HashTagEntity : EntityBase
    {
        /// <summary>
        /// Tag name without the # sign
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// Locations for begin/end index of where hashtag occurs.
        /// </summary>
        [JsonPropertyName("indices")]
        public int[] Indices { get; set; }
    }
}
