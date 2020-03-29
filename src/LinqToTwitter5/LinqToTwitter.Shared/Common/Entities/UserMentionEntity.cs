using Newtonsoft.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter user mention entity in the tweet
    /// </summary>
    /// <example>@JoeMayo</example>
    public class UserMentionEntity : EntityBase
    {
        /// <summary>
        /// Twitter user Id
        /// </summary>
        [JsonProperty("id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Screen name of the Twitter User
        /// </summary>
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        /// <summary>
        /// Name of the Twitter User
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// String version of the ID
        /// </summary>
        [JsonProperty("id_str")]
        public string IdStr { get; set; }

        /// <summary>
        /// Locations for begin/end index of where user mention occurs.
        /// </summary>
        [JsonProperty("indices")]
        public int[] Indices { get; set; }
    }
}
