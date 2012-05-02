//using System.Collections.Generic;
//using System.Runtime.Serialization;

//#if !SILVERLIGHT && !CLIENT_PROFILE
//using System.Web.Script.Serialization;
//#endif

//namespace LinqToTwitter.Json
//{
//    [DataContract]
//    public class SleepTime
//    {
//        [DataMember]
//        public int end_time { get; set; }
//        [DataMember]
//        public bool enabled { get; set; }
//        [DataMember]
//        public int start_time { get; set; }

//        public static SleepTime Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
//        {
//            return new SleepTime
//            {
//                start_time = dictionary.GetValue("start_time", 0),
//                end_time = dictionary.GetValue("end_time", 0),
//                enabled = dictionary.GetValue("enabled", false)
//            };
//        }
//    }
//}
