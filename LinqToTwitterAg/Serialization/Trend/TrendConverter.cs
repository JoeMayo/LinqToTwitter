//using System;
//using System.Collections.Generic;

//#if !SILVERLIGHT && !CLIENT_PROFILE
//using System.Web.Script.Serialization;
//#endif

//namespace LinqToTwitter.Json
//{
//    public class TrendConverter : JavaScriptConverter
//    {
//        public static JavaScriptSerializer GetSerializer()
//        {
//            var converter = new Json.TrendConverter();
//            var jss = new JavaScriptSerializer();
//            jss.RegisterConverters(new JavaScriptConverter[] { converter });
//            return jss;
//        }

//        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
//        {
//            // TODO: Implement this method
//            throw new NotImplementedException();
//        }

//        public override IEnumerable<Type> SupportedTypes
//        {
//            get
//            {
//                return new List<Type> 
//                {
//                    typeof(Json.DailyWeeklyTrends), 
//                    typeof(Json.Trends),
//                    typeof(Json.SlottedTrend),
//                    typeof(Json.Trend),
//                    typeof(Json.Place),
//                    typeof(Json.PlaceType)
//                };
//            }
//        }

//        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
//        {
//            if (dictionary == null)
//                throw new ArgumentNullException("dictionary");

//            if (type == typeof(Json.DailyWeeklyTrends))
//            {
//                return Json.DailyWeeklyTrends.Deserialize(dictionary, serializer);
//            }
//            else if (type == typeof(Json.SlottedTrend))
//            {
//                return Json.SlottedTrend.Deserialize(dictionary, serializer);
//            }
//            else if (type == typeof(Json.Trends))
//            {
//                return Json.Trends.Deserialize(dictionary, serializer);
//            }
//            else if (type == typeof(Json.Trend))
//            {
//                return Json.Trend.Deserialize(dictionary, serializer);
//            }
//            else if (type == typeof(Json.Place))
//            {
//                return Json.Place.Deserialize(dictionary, serializer);
//            }
//            else if (type == typeof(Json.PlaceType))
//            {
//                return Json.PlaceType.Deserialize(dictionary, serializer);
//            }
//            else
//            {
//                throw new ArgumentException("Unsupported Type:" + type.ToString(), "type");
//            }
//        }
//    }
//}
