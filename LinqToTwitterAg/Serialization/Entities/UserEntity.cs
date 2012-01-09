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
    public class UserEntity : EntityBase
    {
        [DataMember]
        public string screen_name { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public long id { get; set; }

        public static UserEntity Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var indices = serializer.ConvertToType<int[]>(dictionary["indices"]);
            return new UserEntity
            {
                screen_name = dictionary.GetValue<string>("screen_name"),
                name = dictionary.GetValue<string>("name"),
                id = dictionary.GetValue<string>("id_str").GetLong(),
                start = indices[0],
                stop = indices[1]
            };
        }
    }
}
