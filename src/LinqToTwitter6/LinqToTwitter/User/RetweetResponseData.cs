using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record RetweetResponseData
    {
        [JsonPropertyName("retweeted")]
        public bool Retweeted { get; set; }
    }
}