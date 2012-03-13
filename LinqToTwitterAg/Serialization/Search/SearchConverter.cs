using System;
using System.Collections.Generic;

#if !SILVERLIGHT && !CLIENT_PROFILE
using System.Web.Script.Serialization;
#endif

namespace LinqToTwitter.Json
{
    public class SearchConverter : JavaScriptConverter
    {
        public static JavaScriptSerializer GetSerializer()
        {
            var converter = new Json.SearchConverter();
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
                    typeof(Json.Search), 
                    typeof(Json.SearchMetaData),
                    typeof(Json.SearchResult),
                    typeof(Json.SearchResult[]),
                    typeof(Json.HashtagEntity),
                    typeof(Json.MediaEntity),
                    typeof(Json.UrlEntity),
                    typeof(Json.UserEntity),
                    typeof(Json.Entities),
                    typeof(Json.Geometry)
                };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(Json.Search))
            {
                return Json.Search.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.SearchMetaData))
            {
                return Json.SearchMetaData.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.SearchResult))
            {
                return Json.SearchResult.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.SearchResult[]))
            {
                return Json.SearchResult.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.HashtagEntity))
            {
                return Json.HashtagEntity.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.MediaEntity))
            {
                return Json.MediaEntity.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.UrlEntity))
            {
                return Json.UrlEntity.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.UserEntity))
            {
                return Json.UserEntity.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.Entities))
            {
                return Json.Entities.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(Json.Geometry))
            {
                return Json.Geometry.Deserialize(dictionary, serializer);
            }
            else
            {
                throw new ArgumentException("Unsupported Type:" + type.ToString(), "type");
            }
        }
    }
}
