using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Type of Tweet Media
    /// </summary>
    public enum TweetMediaType
    {
        /// <summary>
        /// Not set
        /// </summary>
        [JsonPropertyName("")]
        None,

        /// <summary>
        /// GIF Animation
        /// </summary>
        [JsonPropertyName("animated_gif")]
        AnimatedGif,

        /// <summary>
        /// Photo
        /// </summary>
        [JsonPropertyName("photo")]
        Photo,

        /// <summary>
        /// Video
        /// </summary>
        [JsonPropertyName("video")]
        Video
    }
}
