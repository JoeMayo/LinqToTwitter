//using System.Collections.Generic;
//using System.Runtime.Serialization;

//#if !SILVERLIGHT && !CLIENT_PROFILE
//using System.Web.Script.Serialization;
//#endif

//namespace LinqToTwitter.Json
//{
//    [DataContract]
//    public class MediaEntity : EntityBase
//    {
//        [DataMember]
//        public ulong id { get; set; }
//        [DataMember]
//        public string media_url { get; set; }
//        [DataMember]
//        public string media_url_https { get; set; }
//        [DataMember]
//        public string url { get; set; }
//        [DataMember]
//        public string display_url { get; set; }
//        [DataMember]
//        public string expanded_url { get; set; }
//        [DataMember]
//        public string type { get; set; }
//        [DataMember]
//        public ImageSize[] sizes { get; set; }

//        public static MediaEntity Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
//        {
//            var indices = serializer.ConvertToType<int[]>(dictionary["indices"]);

//            var sizeHolder = dictionary["sizes"] as IDictionary<string, object>;
//            var sizeArray = new ImageSize[5];
//            sizeArray[0] = sizeHolder.GetNested<ImageSize>("orig", serializer);
//            sizeArray[0].type = "orig";
//            sizeArray[1] = sizeHolder.GetNested<ImageSize>("thumb", serializer);
//            sizeArray[1].type = "thumb";
//            sizeArray[2] = sizeHolder.GetNested<ImageSize>("large", serializer);
//            sizeArray[2].type = "large";
//            sizeArray[3] = sizeHolder.GetNested<ImageSize>("small", serializer);
//            sizeArray[3].type = "small";
//            sizeArray[4] = sizeHolder.GetNested<ImageSize>("medium", serializer);
//            sizeArray[4].type = "medium";

//            return new MediaEntity
//            {
//                id = dictionary.GetValue<string>("id_str").GetULong(),
//                media_url = dictionary.GetValue<string>("media_url"),
//                media_url_https = dictionary.GetValue<string>("media_url_https"),
//                url = dictionary.GetValue<string>("url"),
//                display_url = dictionary.GetValue<string>("display_url"),
//                expanded_url = dictionary.GetValue<string>("expanded_url"),
//                type = dictionary.GetValue<string>("type"),
//                sizes = sizeArray,
//                start = indices[0],
//                stop = indices[1]
//            };
//        }
//    }
//}
