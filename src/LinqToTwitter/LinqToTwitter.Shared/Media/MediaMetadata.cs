using Newtonsoft.Json;

namespace LinqToTwitter
{
    public class MediaMetadata
    {
        [JsonProperty("media_id")]
        public ulong MediaID { get; set; }
        [JsonProperty("alt_text")]
        public AltText AltText { get; set; }
    }

    public class AltText
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
