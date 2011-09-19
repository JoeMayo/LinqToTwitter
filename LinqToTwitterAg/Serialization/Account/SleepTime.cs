using System.Collections.Generic;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class SleepTime
    {
        [DataMember]
        public int? end_time { get; set; }
        [DataMember]
        public bool enabled { get; set; }
        [DataMember]
        public int? start_time { get; set; }

        public static SleepTime Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new SleepTime
            {
                start_time = dictionary.GetValue<int?>("start_time"),
                end_time = dictionary.GetValue<int?>("end_time"),
                enabled = dictionary.GetValue("enabled", false)
            };
        }
    }
}
