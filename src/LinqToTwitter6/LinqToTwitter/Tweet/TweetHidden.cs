using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetHidden
    {
        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }
    }
}