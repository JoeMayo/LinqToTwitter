using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class GeoScrub
    {
        public GeoScrub() { }
        public GeoScrub(JsonData geo)
        {
            var scrub = geo.GetValue<JsonData>("scrub_geo");
            UserID = scrub.GetValue<ulong>("user_id");
            UpToStatusID = scrub.GetValue<ulong>("up_to_status_id");
        }
        
        public ulong UserID { get; set; }

        public ulong UpToStatusID { get; set; }
    }
}
