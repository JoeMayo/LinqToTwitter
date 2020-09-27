using System.Text.Json;

namespace LinqToTwitter.Common.Entities
{
    public class Variant
    {
        public Variant() { }
        public Variant(JsonElement variant)
        {
            BitRate = variant.GetInt("bitrate");
            ContentType = variant.GetString("content_type");
            Url = variant.GetString("url");
        }

        public int BitRate { get; set; }

        public string? ContentType { get; set; }

        public string? Url { get; set; }
    }
}
