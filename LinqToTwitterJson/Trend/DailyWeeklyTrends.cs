using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace LinqToTwitter.Json
{
    [DataContract]
    public class DailyWeeklyTrends
    {
        [DataMember]
        public ulong as_of { get; set; } // epoch seconds

        [DataMember]
        public Json.SlottedTrend[] slots { get; set; }

        public static DailyWeeklyTrends Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var trends = dictionary["trends"] as Dictionary<string, object>;
            var asOfEpoch = (int)dictionary["as_of"];
            var trendSlots = from trend in trends
                             let slotTime = trend.Key
                             let trendArray = serializer.ConvertToType<Trend[]>(trend.Value)
                             select new SlottedTrend
                             {
                                 time_slot = slotTime,
                                 trends = trendArray
                             };

            return new DailyWeeklyTrends
            {
                as_of = (ulong)asOfEpoch,
                slots = trendSlots.ToArray()
            };
        }
    }
}
