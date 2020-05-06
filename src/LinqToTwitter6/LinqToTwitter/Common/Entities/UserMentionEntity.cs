using System.Text.Json.Serialization;

namespace LinqToTwitter.Common.Entities
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
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Screen name of the Twitter User
        /// </summary>
        [JsonPropertyName("screen_name")]
        public string ScreenName { get; set; }

        /// <summary>
        /// Name of the Twitter User
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// String version of the ID
        /// </summary>
        [JsonPropertyName("id_str")]
        public string IdStr { get; set; }

        /// <summary>
        /// Locations for begin/end index of where user mention occurs.
        /// </summary>
        [JsonPropertyName("indices")]
        public int[] Indices { get; set; }
    }
}
