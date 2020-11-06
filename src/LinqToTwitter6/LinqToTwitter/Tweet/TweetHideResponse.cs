using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetHideResponse
    {
        [JsonPropertyName("data")]
        public TweetHidden? Data { get; set; }
    }
}