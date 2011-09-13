using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace LinqToTwitter.Json
{
    public class AccountConverter : JavaScriptConverter
    {
        public static JavaScriptSerializer GetSerializer()
        {
            var converter = new Json.AccountConverter();
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { converter });
            return jss;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new List<Type> 
                {
                    typeof(Json.Settings), 
                    typeof(Json.SleepTime),
                    typeof(Json.TimeZone),
                    typeof(Json.RateLimitStatus),
                    typeof(Json.User),
                    typeof(Json.Status),
                    typeof(Json.Totals),
                    typeof(Json.Place),
                    typeof(Json.PlaceType)
                };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(Json.Settings))
            {
                return Json.Settings.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.SleepTime))
            {
                return Json.SleepTime.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.TimeZone))
            {
                return Json.TimeZone.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.RateLimitStatus))
            {
                return Json.RateLimitStatus.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.User))
            {
                return Json.User.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.Status))
            {
                return Json.Status.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.Totals))
            {
                return Json.Totals.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.Place))
            {
                return Json.Place.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.PlaceType))
            {
                return Json.PlaceType.Deserialize(dictionary, serializer);
            }
            else
            {
                throw new ArgumentException("Unsupported Type:" + type.ToString(), "type");
            }
        }
    }
}
