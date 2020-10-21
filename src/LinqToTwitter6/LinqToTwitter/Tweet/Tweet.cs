using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record Tweet
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
