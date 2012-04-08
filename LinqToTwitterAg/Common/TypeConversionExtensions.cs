using System;
using System.Globalization;
using System.Xml.Linq;
using System.Collections.Generic;

using LitJson;

namespace LinqToTwitter.Common
{
    public static class TypeConversionExtensions
    {
        internal static string TagValue(this XElement elem, XName name)
        {
            if (elem == null)
                return null;

            var val = name == null ? elem : elem.Element(name);
            return val == null ? null : val.Value;
        }

        public static bool GetBool(this XElement elem, XName name)
        {
            return elem.GetBool(name, default(bool));
        }

        public static bool GetBool(this XElement elem, XName name, bool defaultValue /* = false*/)
        {
            var val = elem.TagValue(name);
            return val.GetBool(defaultValue);
        }

        public static bool GetBool(this string val, bool defaultValue /* = false*/)
        {
            bool result;

            return String.IsNullOrEmpty(val) ||
                !bool.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static int GetInt(this XElement elem, XName name)
        {
            return elem.GetInt(name, default(int));
        }

        public static int GetInt(this XElement elem, XName name, int defaultValue /*= 0*/)
        {
            var val = elem.TagValue(name);
            return val.GetInt(defaultValue);
        }

        public static int GetInt(this string val, int defaultValue /* = 0*/)
        {
            int result;
            return String.IsNullOrEmpty(val) ||
                !int.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static ulong GetULong(this XElement elem, XName name)
        {
            return elem.GetULong(name, default(ulong));
        }

        public static ulong GetULong(this XElement elem, XName name, ulong defaultValue /* = 0*/)
        {
            var val = elem.TagValue(name);
            return val.GetULong(defaultValue);
        }

        public static ulong GetULong(this string val, ulong defaultValue /* = 0*/)
        {
            ulong result;

            return String.IsNullOrEmpty(val) ||
                !ulong.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static double GetDouble(this XElement elem, XName name)
        {
            return elem.GetDouble(name, default(double));
        }

        public static double GetDouble(this XElement elem, XName name, double defaultValue /* = 0*/)
        {
            var val = elem.TagValue(name);
            return val.GetDouble(defaultValue);
        }

        public static double GetDouble(this string val, double defaultValue /* = 0*/)
        {
            double result;

            return String.IsNullOrEmpty(val) ||
                !double.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static string GetString(this XElement elem, XName tagName)
        {
            return elem.GetString(tagName, String.Empty);
        }

        public static string GetString(this XElement elem, XName tagName, string defaultValue /* = ""*/)
        {
            var val = elem.TagValue(tagName);

            return string.IsNullOrEmpty(val)
                    ? defaultValue
                    : val;
        }

        public static DateTime GetDate(this XElement elem, XName name)
        {
            return elem.GetDate(name, DateTime.MinValue);
        }

        public static DateTime GetDate(this XElement elem, XName name, DateTime defaultValue)
        {
            var val = elem.TagValue(name);

            return val.GetDate(defaultValue);
        }

        private static readonly string[] DateFormats = { "ddd MMM dd HH:mm:ss %zzzz yyyy",
                                                         "yyyy-MM-dd\\THH:mm:ss\\Z",
                                                         "yyyy-MM-dd HH:mm:ss",
                                                         "yyyy-MM-dd HH:mm"};

        public static DateTime GetDate(this string date, DateTime defaultValue)
        {
            DateTime result;

            return String.IsNullOrEmpty(date) ||
                !DateTime.TryParseExact(date,
                        DateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result)
                    ? defaultValue
                    : result;
        }

        public static readonly DateTime EpochBase = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime GetEpochDate(this string date, DateTime defaultValue)
        {
            var epochSeconds = date.GetULong(ulong.MaxValue);

            if (epochSeconds == ulong.MaxValue)
                return defaultValue;
            
            return EpochBase + TimeSpan.FromSeconds(epochSeconds);
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;

            if (dictionary.TryGetValue(key, out value))
                return (T)value;

            return default(T);
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            object value;

            if (dictionary.TryGetValue(key, out value))
                return (T)value;
            
            return defaultValue;
        }

        public static T GetValue<T>(this JsonData data, string key)
        {
            return GetValue(data, key, default(T));
        }

        public static T GetValue<T>(this JsonData data, string key, T defaultValue)
        {
            object value = defaultValue;
            if (data != null && data.InstObject != null && 
                data.InstObject.ContainsKey(key) && data.InstObject[key] != null)
            {
                var dataItem = data.InstObject[key] as IJsonWrapper;

                string type = typeof (T).Name;
                switch (type)
                {
                    case "String":
                        value = dataItem.GetString();
                        break;
                    case "Int32":
                        value = dataItem.GetInt();
                        break;
                    case "Int64":
                        value = dataItem.GetLong();
                        break;
                    case "Double":
                        value = dataItem.GetDouble();
                        break;
                    case "Boolean":
                        value = dataItem.GetBoolean();
                        break;
                    case "Decimal":
                        value = dataItem.GetDecimal();
                        break;
                    case "UInt64":
                        value = dataItem.GetUlong();
                        break;
                    case "JsonData":
                        value = data.InstObject[key];
                        break;
                }
            }

            return (T)value;
        }
    }
}
