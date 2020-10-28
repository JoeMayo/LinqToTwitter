using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TweetGeo
    {
        [JsonPropertyName("place_id")]
        public string? PlaceID { get; set; }
    }
}
