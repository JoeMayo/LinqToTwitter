using System.Text.Json;

namespace LinqToTwitter
{
    public class Limit
    {
        public Limit() { }
        public Limit(JsonElement json)
        {
            var scrub = json.GetProperty("limit");
            Track = scrub.GetProperty("track").GetUInt64();
        }

        public ulong Track { get; set; }
    }
}
