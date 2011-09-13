using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace LinqToTwitter.Json
{
    public static class DictionaryExtensions
    {
        public static DateTime GetValue(this IDictionary<string, object> dictionary, string key, DateTime defaultValue)
        {
            var value = dictionary.GetValue(key, String.Empty);
            return value.GetDate(defaultValue);
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            return dictionary.GetValue(key, default(T));
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            object value;

            if (dictionary.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }

        public static T GetNested<T>(this IDictionary<string, object> dictionary, string key, JavaScriptSerializer serializer)
            where T : class
        {
            var nestedObject = dictionary.GetValue<object>(key, null);

            if (nestedObject != null)
            {
                return serializer.ConvertToType<T>(nestedObject);
            }

            return default(T);
        }

        public static IEnumerable<T> GetNestedEnumeration<T>(this IDictionary<string, object> dictionary, string key, JavaScriptSerializer serializer)
            where T : class
        {
            var array = dictionary.GetValue<ArrayList>(key, null);

            if (array != null)
            {
                var elements = (from object element in array
                                select serializer.ConvertToType<T>(element));
                return elements;
            }

            return Enumerable.Empty<T>();
        }
    }
}
