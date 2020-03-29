using Newtonsoft.Json;
using System.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Url mention in the tweet
    /// </summary>
    /// <example>http://bit.ly/129Ad</example>
    public class UrlEntity : EntityBase
    {
        /// <summary>
        /// Absolute Url in the tweet
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// t.co shortened URL
        /// </summary>
        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }

        /// <summary>
        /// t.co expanded URL
        /// </summary>
        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }

        /// <summary>
        /// Locations for begin/end index of where URL occurs.
        /// </summary>
        [JsonProperty("indices")]
        public int[] Indices { get; set; }
    }
}
