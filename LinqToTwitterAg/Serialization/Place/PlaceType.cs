using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinqToTwitter.Json
{
    [DataContract]
    public class PlaceType
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int code { get; set; }

        public static PlaceType Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new PlaceType
            {
                name = dictionary["name"] as string,    // required!
                code = dictionary.GetValue<int>("code")
            };
        }
    }
}
