using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class SlottedTrend
    {
        [DataMember]
        public string time_slot { get; set; }

        [DataMember]
        public Json.Trend[] trends { get; set; }

        public static SlottedTrend Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            // THIS IS WRONG, fortunately we never need to do this...
            var slot = dictionary.First();
            var slotTime = slot.Key;
            var trendArray = serializer.ConvertToType<Json.Trend[]>(slot.Value);
            return new SlottedTrend
            {
                time_slot = slotTime,
                trends = trendArray
            };
        }
    }
}
