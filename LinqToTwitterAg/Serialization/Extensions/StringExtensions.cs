using System;
using System.Globalization;

namespace LinqToTwitter.Json
{
    public static class StringExtensions
    {
        public static bool GetBool(this string val)
        {
            return GetBool(val, false);
        }

        public static bool GetBool(this string val, bool defaultValue /* = false*/)
        {
            bool result;

            return String.IsNullOrEmpty(val) ||
                !bool.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static int GetInt(this string val)
        {
            return GetInt(val, 0);
        }

        public static int GetInt(this string val, int defaultValue /* = 0*/)
        {
            int result;
            return String.IsNullOrEmpty(val) ||
                !int.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static long GetLong(this string val)
        {
            return GetLong(val, 0l);
        }

        public static long GetLong(this string val, long defaultValue /* = 0*/)
        {
            long result;

            return String.IsNullOrEmpty(val) ||
                !long.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static ulong GetULong(this string val)
        {
            return GetULong(val, 0ul);
        }

        public static ulong GetULong(this string val, ulong defaultValue /* = 0*/)
        {
            ulong result;

            return String.IsNullOrEmpty(val) ||
                !ulong.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static double GetDouble(this string val)
        {
            return GetDouble(val, 0d);
        }

        public static double GetDouble(this string val, double defaultValue /* = 0*/)
        {
            double result;

            return String.IsNullOrEmpty(val) ||
                !double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                    ? defaultValue
                    : result;
        }


        public static DateTime GetDate(this string date)
        {
            return GetDate(date, DateTime.MinValue);
        }

        public static readonly string[] DateFormats = { "ddd MMM dd HH:mm:ss %zzzz yyyy",
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
            else
                return EpochBase + TimeSpan.FromSeconds(epochSeconds);
        }
    }
}
