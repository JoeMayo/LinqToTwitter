using System.Text.Json;

namespace LinqToTwitter
{
    public class GeoScrub
    {
        public GeoScrub() { }
        public GeoScrub(JsonElement geo)
        {
            var scrub = geo.GetProperty("scrub_geo");
            UserID = scrub.GetProperty("user_id").GetUInt64();
            UpToStatusID = scrub.GetProperty("up_to_status_id").GetUInt64();
        }
        
        public ulong UserID { get; set; }

        public ulong UpToStatusID { get; set; }
    }
}
