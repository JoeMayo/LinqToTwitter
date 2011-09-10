using System.Collections.Generic;

namespace LinqToTwitter.Json
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;

            if (dictionary.TryGetValue(key, out value))
                return (T)value;
            else
                return default(T);
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            object value;

            if (dictionary.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }
    }
}
