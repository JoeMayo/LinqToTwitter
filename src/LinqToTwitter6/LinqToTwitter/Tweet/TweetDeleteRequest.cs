using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetDeleteRequest
    {
        [JsonPropertyName("id")]
        public string? ID { get; set; }
    }
}
