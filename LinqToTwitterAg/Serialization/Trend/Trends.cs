using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

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
            var trds = dictionary.GetNestedEnumeration<Trend>("trends", serializer);
            var locs = dictionary.GetNestedEnumeration<Place>("locations", serializer);
            
            return new Trends
            {
                as_of = dictionary.GetValue<string>("as_of"),
                created_at = dictionary.GetValue<string>("created_at"),
                locations = locs.ToArray(),
                trends = trds.ToArray()
            };
        }
    }
}
