using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Place
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public ulong woeid { get; set; }
        [DataMember]
        public ulong parentid { get; set; }
        [DataMember]
        public PlaceType placeType { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string countryCode { get; set; }

        public static Place Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var placeName = dictionary["name"] as string; // required!
            var pt = dictionary.GetNested<PlaceType>("placeType", serializer);

            return new Place
            {
                name = placeName,
                url = dictionary.GetValue<string>("url"),
                woeid = (ulong)dictionary.GetValue<int>("woeid"),
                parentid = (ulong)dictionary.GetValue<int>("parentid"),
                placeType = pt,
                country = dictionary.GetValue<string>("country"),
                countryCode = dictionary.GetValue<string>("countryCode")
            };
        }
    }
}
