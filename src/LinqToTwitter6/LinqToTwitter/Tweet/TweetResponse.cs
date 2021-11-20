using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetResponse
    {
        [JsonPropertyName("data")]
        public Tweet? Tweet { get; set; }
    }
}
