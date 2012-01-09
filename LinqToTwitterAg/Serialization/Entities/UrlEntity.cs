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
    public class UrlEntity : EntityBase
    {
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string expanded_url { get; set; }
        [DataMember]
        public string display_url { get; set; }

        public static UrlEntity Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var indices = serializer.ConvertToType<int[]>(dictionary["indices"]);
            return new UrlEntity
            {
                url = dictionary.GetValue<string>("url"),
                expanded_url = dictionary.GetValue<string>("expanded_url"),
                display_url = dictionary.GetValue<string>("display_url"),
                start = indices[0],
                stop = indices[1]
            };
        }
    }
}
