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
    public class Geometry
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public double latitude { get; set; }
        [DataMember]
        public double longitude { get; set; }

        public static Geometry Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var coordinates = serializer.ConvertToType<string[]>(dictionary["coordinates"]);
            return new Geometry
            {
                type = dictionary.GetValue<string>("type"),
                latitude = coordinates[0].GetDouble(),
                longitude = coordinates[1].GetDouble()
            };
        }
    }
}
