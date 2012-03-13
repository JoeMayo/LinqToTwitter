using System.Collections.Generic;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class SearchMetaData
    {
        [DataMember]
        public int recent_retweets { get; set; }

        [DataMember]
        public string result_type { get; set; }

        public static SearchMetaData Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new SearchMetaData
            {
                recent_retweets = dictionary.GetValue<int>("recent_retweets"),
                result_type = dictionary.GetValue<string>("result_type"),
            };
        }
    }
}
