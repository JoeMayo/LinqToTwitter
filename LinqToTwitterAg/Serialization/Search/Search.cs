using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class Search
    {
        [DataMember]
        public decimal completed_in { get; set; }
        [DataMember]
        public ulong max_id { get; set; }
        [DataMember]
        public string max_id_str { get; set; }
        [DataMember]
        public string next_page { get; set; }
        [DataMember]
        public int page { get; set; }
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public string refresh_url { get; set; }
        [DataMember]
        public SearchResult[] results { get; set; }
        [DataMember]
        public int results_per_page { get; set; }
        [DataMember]
        public ulong since_id { get; set; }

        public static Search Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var rslts = dictionary.GetNestedEnumeration<SearchResult>("results", serializer);
            return new Search
            {
                completed_in = dictionary.GetValue<decimal>("completed_in"),
                max_id = dictionary.GetValue<ulong>("max_id"),
                max_id_str = dictionary.GetValue<string>("max_id_str"),
                next_page = dictionary.GetValue<string>("next_page"),
                page = dictionary.GetValue<int>("page"),
                query = dictionary.GetValue<string>("query"),
                refresh_url = dictionary.GetValue<string>("refresh_url"),
                results = rslts.ToArray(),
                results_per_page = dictionary.GetValue<int>("results_per_page"),
                since_id = dictionary.GetValue<ulong>("since_id_str")
            };
        }
    }
}
