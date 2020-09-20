using System;
using System.Globalization;
using System.Text.Json;

namespace LinqToTwitter.Common
{
    public static class TypeConversionExtensions
    {
        public static bool IsNull(this JsonElement json)
        {
            return json.ValueKind == JsonValueKind.Undefined || json.ValueKind == JsonValueKind.Null;
        }

        public static string? GetString(this JsonElement json, string propertyName, string? defaultValue = default)
        {
            if (!json.IsNull() && 
                json.TryGetProperty(propertyName, out JsonElement element))
                return element.GetString() ?? defaultValue;

            return defaultValue;
        }

        public static int GetInt(this JsonElement json, string propertyName, int defaultValue = default)
        {
            if (!json.IsNull() && 
                json.TryGetProperty(propertyName, out JsonElement element) &&
                !element.IsNull() &&
                element.TryGetInt32(out int value))
                return value;

            return defaultValue;
        }

        public static ulong GetULong(this string val, ulong defaultValue = default)
        {
            return string.IsNullOrWhiteSpace(val) ||
                !ulong.TryParse(val, out ulong result)
                    ? defaultValue
                    : result;
        }

        public static ulong GetUlong(this JsonElement json, string propertyName, ulong defaultValue = default)
        {
            if (!json.IsNull() && 
                json.TryGetProperty(propertyName, out JsonElement element) &&
                !element.IsNull() &&
                element.TryGetUInt64(out ulong value))
                return value;

            return defaultValue;
        }

        public static long GetLong(this JsonElement json, string propertyName, long defaultValue = default)
        {
            if (!json.IsNull() &&
                json.TryGetProperty(propertyName, out JsonElement element) &&
                !element.IsNull() &&
                element.TryGetInt64(out long value))
                return value;

            return defaultValue;
        }

        public static bool GetBool(this JsonElement json, string propertyName, bool defaultValue = default)
        {
            if (!json.IsNull() && json.TryGetProperty(propertyName, out JsonElement element))
                return element.GetBoolean();

            return defaultValue;
        }

        public static double GetDouble(this string val, double defaultValue = default)
        {
            return string.IsNullOrWhiteSpace(val) ||
                !double.TryParse(val, out double result)
                    ? defaultValue
                    : result;
        }

        public static double GetDouble(this JsonElement json, string propertyName, double defaultValue = default)
        {
            if (!json.IsNull() && 
                json.TryGetProperty(propertyName, out JsonElement element) &&
                !element.IsNull() &&
                element.TryGetDouble(out double value))
                return value;

            return defaultValue;
        }

        public static decimal GetDecimal(this JsonElement json, string propertyName, decimal defaultValue = default)
        {
            if (!json.IsNull() &&
                json.TryGetProperty(propertyName, out JsonElement element) &&
                !element.IsNull() &&
                element.TryGetDecimal(out decimal value))
                return value;

            return defaultValue;
        }

        static readonly string[] dateFormats =
        {
            "ddd MMM dd HH:mm:ss %zzzz yyyy",
            "yyyy-MM-dd\\THH:mm:ss\\Z",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm"
        };

        public static DateTime GetDate(this string date, DateTime defaultValue)
        {
            return string.IsNullOrWhiteSpace(date) ||
                !DateTime.TryParseExact(date,
                        dateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime result)
                    ? defaultValue
                    : result;
        }

        public static DateTime GetDate(this JsonElement json, string propertyName, DateTime defaultValue = default)
        {
            string? date = json.GetString(propertyName);

            return
                string.IsNullOrWhiteSpace(date) ||
                !DateTime.TryParseExact(
                        date,
                        dateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime result)
                    ? defaultValue
                    : result;
        }

        public static readonly DateTime EpochBase = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime GetEpochDate(this string ticks, DateTime defaultValue)
        {
            var epochSeconds = ticks.GetULong(ulong.MaxValue);

            if (epochSeconds == ulong.MaxValue)
                return defaultValue;

            return EpochBase + TimeSpan.FromSeconds(epochSeconds);
        }

        public static DateTime GetEpochDateFromTimestamp(this string timestamp)
        {
            ulong.TryParse(timestamp, out ulong epochMilliseconds);
            return EpochBase + +TimeSpan.FromMilliseconds(epochMilliseconds);
        }
    }
}
