#nullable disable
using System.Text.Json;

namespace LinqToTwitter.Common.Entities
{
    public class Variant
    {
        public Variant() { }
        public Variant(JsonElement variant)
        {
            BitRate = variant.GetProperty("bitrate").GetInt32();
            ContentType = variant.GetProperty("content_type").GetString();
            Url = variant.GetProperty("url").GetString();
        }

        public int BitRate { get; set; }

        public string ContentType { get; set; }

        public string Url { get; set; }
    }
}
