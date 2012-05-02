//using System;
//using System.Collections.Generic;
//using System.Runtime.Serialization;

//#if !SILVERLIGHT && !CLIENT_PROFILE
//using System.Web.Script.Serialization;
//#endif

//namespace LinqToTwitter.Json
//{
//    [DataContract]
//    public class Status
//    {
//        [DataMember]
//        public ulong id { get; set; }
//        [DataMember]
//        public bool truncated { get; set; }
//        [DataMember]
//        public bool favorited { get; set; }
//        [DataMember]
//        public bool retweeted { get; set; }
//        [DataMember]
//        public ulong in_reply_to_status_id { get; set; }
//        [DataMember]
//        public ulong in_reply_to_user_id { get; set; }
//        [DataMember]
//        public string in_reply_to_screen_name { get; set; }
//        [DataMember]
//        public User user { get; set; }
//        [DataMember]
//        public DateTime created_at { get; set; }
//        [DataMember]
//        public int retweet_count { get; set; } // twitter adds a + sometimes, we strip in processing
//        [DataMember]
//        public string text { get; set; }
//        [DataMember]
//        public string source { get; set; }
//        /*
//        [DataMember]
//        public Geo geo { get; set; }
//        */
//        [DataMember]
//        public string coordinates { get; set; }
//        [DataMember]
//        public Place place { get; set; }
//        /*
//          "entities": {
//            "places": [
         
//            ],
//            "urls": [
//              {
//                "expanded_url": "http://post.ly/1TDKK",
//                "url": "http://t.co/o9QIqTH",
//                "indices": [
//                  38,
//                  57
//                ],
//                "display_url": "post.ly/1TDKK"
//              }
//            ],
//            "hashtags": [
         
//            ],
//            "user_mentions": [
         
//            ]
//          },
//          "annotations": null,
//          "contributors": null,
//         */

//        public static Status Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
//        {
//            var id_str = dictionary.GetValue<String>("id_str");
//            var id = id_str.GetULong(0UL);
//            var truncated = dictionary.GetValue("truncated", false);
//            var favorited = dictionary.GetValue("favorited", false);
//            var retweeted = dictionary.GetValue("retweeted", false);
//            var in_reply_to_status_id_str = dictionary.GetValue("in_reply_to_status_id_str", "0");
//            var in_reply_to_status_id = in_reply_to_status_id_str.GetULong(0UL);
//            var in_reply_to_user_id_str = dictionary.GetValue("in_reply_to_user_id_str", "0");
//            var in_reply_to_user_id = in_reply_to_user_id_str.GetULong(0UL);
//            var in_reply_to_screen_name = dictionary.GetValue("in_reply_to_screen_name", String.Empty);
//            var user = dictionary.GetNested<User>("user", serializer);
//            var created_at = dictionary.GetValue("created_at", DateTime.MinValue);
//            var retweet_count_str = dictionary.GetValue("retweet_count_str", "0"); // must not be int, because twitter adds a + sometimes
//            var retweet_count = retweet_count_str.TrimEnd('+').GetInt(0);
//            var text = dictionary.GetValue<String>("text");
//            var source = dictionary.GetValue<String>("source");
//            //var coordinates = dictionary.GetValue<String>("coordinates");
//            //var place = dictionary.GetNested<Place>("place", serializer);
//            //var geo = dictionary.GetNested<Geo>("geo", serializer);
//            //var entities = dictionary.GetNested<Entities>("entities", serializer);

//            return new Status
//            {
//                id = id,
//                truncated = truncated,
//                favorited = favorited,
//                retweeted = retweeted,
//                in_reply_to_status_id = in_reply_to_status_id,
//                in_reply_to_user_id = in_reply_to_user_id,
//                in_reply_to_screen_name = in_reply_to_screen_name,
//                created_at = created_at,
//                retweet_count = retweet_count,
//                text = text,
//                source = source,
//                user = user,
//                //geo = geo,
//                //coordinates = coordinates,
//                //place = place,
//                // entities = entities
//            };
//        }
//    }
//}
