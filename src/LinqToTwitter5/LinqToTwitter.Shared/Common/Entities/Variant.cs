using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Variant
    {
        public Variant() { }
        public Variant(JsonData variant)
        {
            BitRate = variant.GetValue<int>("bitrate", defaultValue: 0);
            ContentType = variant.GetValue<string>("content_type");
            Url = variant.GetValue<string>("url");
        }

        public int BitRate { get; set; }

        public string ContentType { get; set; }

        public string Url { get; set; }
    }
}
