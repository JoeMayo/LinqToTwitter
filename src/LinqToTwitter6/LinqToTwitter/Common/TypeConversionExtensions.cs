#nullable disable
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json;

namespace LinqToTwitter.Common
{
    public static class TypeConversionExtensions
    {

        public static ulong GetULong(this string val, ulong defaultValue /* = 0*/)
        {
            return string.IsNullOrWhiteSpace(val) ||
                !ulong.TryParse(val, out ulong result)
                    ? defaultValue
                    : result;
        }

        public static double GetDouble(this string val, double defaultValue /* = 0*/)
        {
            return String.IsNullOrWhiteSpace(val) ||
                !double.TryParse(val, out double result)
                    ? defaultValue
                    : result;
        }

        private static readonly string[] dateFormats = { "ddd MMM dd HH:mm:ss %zzzz yyyy",
                                                         "yyyy-MM-dd\\THH:mm:ss\\Z",
                                                         "yyyy-MM-dd HH:mm:ss",
                                                         "yyyy-MM-dd HH:mm"};

        public static DateTime GetDate(this string date, DateTime defaultValue)
        {
            return String.IsNullOrWhiteSpace(date) ||
                !DateTime.TryParseExact(date,
                        dateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime result)
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

        public static DateTime GetEpochDateFromTimestamp(this string timestamp)
        {
            ulong.TryParse(timestamp, out ulong epochMilliseconds);
            return EpochBase + +TimeSpan.FromMilliseconds(epochMilliseconds);
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object value))
                return (T)value;

            return default;
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {

            if (dictionary.TryGetValue(key, out object value))
                return (T)value;

            return defaultValue;
        }

        public static T GetValue<T>(this JsonDocument data, string key)
        {
            return GetValue(data, key, default(T));
        }

        public static T GetValue<T>(this JsonDocument data, string key, T defaultValue)
        {
            throw new NotImplementedException();
            //object value = defaultValue;
            //if (data != null && data.InstObject != null && 
            //    data.InstObject.ContainsKey(key) && data.InstObject[key] != null)
            //{
            //    var dataItem = data.InstObject[key] as IJsonWrapper;

            //    string type = typeof (T).Name;
            //    switch (type)
            //    {
            //        case "String":
            //            value = dataItem.GetString();
            //            break;
            //        case "Int32":
            //            value = dataItem.GetInt();
            //            break;
            //        case "Int64":
            //            value = dataItem.GetLong();
            //            break;
            //        case "Double":
            //            value = dataItem.GetDouble();
            //            break;
            //        case "Boolean":
            //            value = dataItem.GetBoolean();
            //            break;
            //        case "Decimal":
            //            value = dataItem.GetDecimal();
            //            break;
            //        case "UInt64":
            //            value = dataItem.GetUlong();
            //            break;
            //        case "JsonData":
            //            value = data.InstObject[key];
            //            break;
            //        case "Nullable`1":
            //            if (typeof(T) == typeof(int?) && dataItem.IsInt) { value = dataItem.GetInt(); break; }
            //            if (typeof(T) == typeof(long?) && dataItem.IsLong) { value = dataItem.GetLong(); break; }
            //            if (typeof(T) == typeof(double?) && dataItem.IsDouble) { value = dataItem.GetDouble(); break; }
            //            if (typeof(T) == typeof(bool?) && dataItem.IsBoolean) { value = dataItem.GetBoolean(); break; }
            //            if (typeof(T) == typeof(decimal?) && dataItem.IsDecimal) { value = dataItem.GetDecimal(); break; }
            //            if (typeof(T) == typeof(ulong?) && dataItem.IsULong) { value = dataItem.GetUlong(); break; }
            //            break;
            //    }
            //}

            //return (T)value;
        }
    }
}
