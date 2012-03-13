using System.Collections.Generic;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class HashtagEntity : EntityBase
    {
        [DataMember]
        public string text { get; set; }

        public static HashtagEntity Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var indices = serializer.ConvertToType<int[]>(dictionary["indices"]);
            return new HashtagEntity
            {
                text = dictionary.GetValue<string>("text"),
                start = indices[0],
                stop = indices[1]
            };
        }
    }
}
