//using System;
//using System.Collections.Generic;
//using System.Runtime.Serialization;

//#if !SILVERLIGHT && !CLIENT_PROFILE
//using System.Web.Script.Serialization;
//#endif

//namespace LinqToTwitter.Json
//{
//    [DataContract]
//    public class RateLimitStatus
//    {
//        [DataMember]
//        public int remaining_hits { get; set; }
//        [DataMember]
//        public int reset_time_in_seconds { get; set; }
//        [DataMember]
//        public int hourly_limit { get; set; }
//        [DataMember]
//        public DateTime reset_time { get; set; }

//        public static RateLimitStatus Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
//        {
//            return new RateLimitStatus
//            {
//                remaining_hits = dictionary.GetValue<int>("remaining_hits"),
//                reset_time_in_seconds = dictionary.GetValue<int>("reset_time_in_seconds"),
//                hourly_limit = dictionary.GetValue<int>("hourly_limit"),
//                reset_time = dictionary.GetValue("reset_time", DateTime.MaxValue)
//            };
//        }
//    }
//}
