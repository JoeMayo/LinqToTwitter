using Newtonsoft.Json;

namespace LinqToTwitter
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
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Locations for begin/end index of where hashtag occurs.
        /// </summary>
        [JsonProperty("indices")]
        public int[] Indices { get; set; }
    }
}
