using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Trend
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string events { get; set; }  // no idea
        [DataMember]
        public string promoted_content { get; set; } // not sure

        public static Trend Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new Trend
            {
                name = dictionary["name"] as string, // required!
                query = dictionary.GetValue<string>("query"),
                url = dictionary.GetValue<string>("url"),
                events = dictionary.GetValue<string>("events"),
                promoted_content = dictionary.GetValue<string>("promoted_content")
            };
        }
    }
}
