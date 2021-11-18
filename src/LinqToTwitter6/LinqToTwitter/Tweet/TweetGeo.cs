using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical location of tweet
    /// </summary>
    public record TweetGeo
    {
        /// <summary>
        /// Place ID for location of tweet
        /// </summary>
        [JsonPropertyName("place_id")]
        public string? PlaceID { get; init; }
    }
}
