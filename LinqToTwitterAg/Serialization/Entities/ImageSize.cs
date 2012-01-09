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
    public class ImageSize
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int w { get; set; }
        [DataMember]
        public int h { get; set; }
        [DataMember]
        public string resize { get; set; }

        public static ImageSize Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new ImageSize
            {
                w = dictionary.GetValue<int>("w"),
                h = dictionary.GetValue<int>("h"),
                resize = dictionary.GetValue<string>("resize")
            };
        }

    }
}
