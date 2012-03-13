using System.Collections.Generic;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Totals
    {
        [DataMember]
        public int friends { get; set; }
        [DataMember]
        public int updates { get; set; }
        [DataMember]
        public int followers { get; set; }
        [DataMember]
        public int favorites { get; set; }

        public static Totals Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new Totals
            {
                friends = dictionary.GetValue<int>("friends"),
                updates = dictionary.GetValue<int>("updates"),
                followers = dictionary.GetValue<int>("followers"),
                favorites = dictionary.GetValue<int>("favorites")
            };
        }
    }
}
