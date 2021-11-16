using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class MediaMetadata
    {
        [JsonPropertyName("media_id")]
        public ulong MediaID { get; set; }
        [JsonPropertyName("alt_text")]
        public AltText? AltText { get; set; }
    }

    public class AltText
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
