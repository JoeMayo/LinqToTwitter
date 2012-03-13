using System.Collections.Generic;
using System.Runtime.Serialization;


#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public string created_at { get; set; }
        [DataMember]
        public Entities entities { get; set; }
        [DataMember]
        public string from_user { get; set; }
        [DataMember]
        public ulong from_user_id { get; set; }
        [DataMember]
        public string from_user_name { get; set; }
        [DataMember]
        public Geometry geo { get; set; }
        [DataMember]
        public ulong id { get; set; }
        [DataMember]
        public string iso_language_code { get; set; }
        [DataMember]
        public SearchMetaData metadata { get; set; }
        [DataMember]
        public string profile_image_url { get; set; }
        [DataMember]
        public string profile_image_url_https { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string to_user { get; set; }
        [DataMember]
        public ulong to_user_id { get; set; }
        [DataMember]
        public string to_user_name { get; set; }

        public static SearchResult Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            // note: use "_id_str" values, such as from_user_id_str, as they contain full
            //       ID as opposed to legacy "_id" values, which could be truncated.

            return new SearchResult
            {
                created_at = dictionary.GetValue<string>("created_at"),
                entities = dictionary.GetNested<Entities>("entities", serializer),
                from_user = dictionary.GetValue<string>("from_user"),
                from_user_id = dictionary.GetValue<string>("from_user_id_str").GetULong(),
                from_user_name = dictionary.GetValue<string>("from_user_name"),
                geo = dictionary.GetNested<Geometry>("geo", serializer),
                id = dictionary.GetValue<string>("id_str").GetULong(0ul),
                iso_language_code = dictionary.GetValue<string>("iso_language_code"),
                metadata = dictionary.GetNested<SearchMetaData>("metadata", serializer),
                profile_image_url = dictionary.GetValue<string>("profile_image_url"),
                profile_image_url_https = dictionary.GetValue<string>("profile_image_url_https"),
                source = dictionary.GetValue<string>("source"),
                text = dictionary.GetValue<string>("text"),
                to_user = dictionary.GetValue<string>("to_user"),
                to_user_id = dictionary.GetValue<string>("to_user_id_str").GetULong(),
                to_user_name = dictionary.GetValue<string>("to_user_name")
            };
        }
    }
}
