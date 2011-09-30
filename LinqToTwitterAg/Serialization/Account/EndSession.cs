using System.Collections.Generic;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    /// <summary>
    /// End Session response
    /// </summary>
    [DataContract]
    public class EndSession
    {
        [DataMember]
        public string request { get; set; }
        [DataMember]
        public string error { get; set; }

        public static EndSession Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new EndSession
            {
                request = dictionary.GetValue<string>("request"),
                error = dictionary.GetValue<string>("error")
            };
        }
    }
}
