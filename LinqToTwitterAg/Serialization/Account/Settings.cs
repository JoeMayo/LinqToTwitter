using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public SleepTime sleep_time { get; set; }
        [DataMember]
        public Place trend_location { get; set; }
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public bool always_use_https { get; set; }
        [DataMember]
        public bool discoverable_by_email { get; set; }
        [DataMember]
        public TimeZone time_zone { get; set; }
        [DataMember]
        public bool geo_enabled { get; set; }

        public static Settings Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var sleepTime = dictionary.GetNested<SleepTime>("sleep_time", serializer);
            var trendLocations = dictionary.GetNestedEnumeration<Place>("trend_location", serializer);
            var timeZone = dictionary.GetNested<TimeZone>("time_zone", serializer);

            return new Settings
            {
                sleep_time = sleepTime,
                trend_location= trendLocations.FirstOrDefault(),
                language = dictionary.GetValue("language", String.Empty),
                always_use_https = dictionary.GetValue("alwaysUseHttps", false),
                discoverable_by_email = dictionary.GetValue("discoverable_by_email", false),
                time_zone = timeZone,
                geo_enabled = dictionary.GetValue("geo_enabled", false)
            };
        }
    }
}
