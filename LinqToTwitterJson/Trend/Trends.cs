using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Trends
    {
        [DataMember]
        public Trend[] trends { get; set; }
        [DataMember]
        public string created_at { get; set; }
        [DataMember]
        public string as_of { get; set; }
        [DataMember]
        public Place[] locations { get; set; }

        public static Trends Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var jsonTrends = dictionary["trends"] as ArrayList;    // required!
            var createdAt = dictionary.GetValue<string>("created_at");
            var asOf = dictionary.GetValue<string>("as_of");
            var locations = dictionary.GetValue<ArrayList>("locations");
            var locs = (from object location in locations
                        select serializer.ConvertToType<Place>(location));
            var trds = (from object trend in jsonTrends
                        select serializer.ConvertToType<Trend>(trend));
            return new Trends
            {
                as_of = asOf,
                created_at = createdAt,
                locations = locs.ToArray(),
                trends = trds.ToArray()
            };
        }
    }
}
