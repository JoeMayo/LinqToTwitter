using System;
using System.Globalization;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public static class XTwitterElement
    {
        private static string TagValue(this XElement elem, string tagName)
        {
            if (elem == null)
                return null;

            var val = elem.Element(tagName);
            return val == null ? null : val.Value;
        }

        public static bool GetBool(this XElement elem, string tagName)
        {
            return elem.GetBool(tagName, default(bool));
        }

        public static bool GetBool(this XElement elem, string tagName, bool defaultValue /* = false*/)
        {
            bool result;
            var val = elem.TagValue(tagName);

            return String.IsNullOrEmpty(val) ||
                !bool.TryParse(val, out result)
                    ? defaultValue
                    : bool.Parse(elem.Element(tagName).Value);
        }

        public static int GetInt(this XElement elem, string tagName)
        {
            return elem.GetInt(tagName, default(int));
        }

        public static int GetInt(this XElement elem, string tagName, int defaultValue /*= 0*/)
        {
            int result;
            var val = elem.TagValue(tagName);

            return String.IsNullOrEmpty(val) ||
                !int.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static ulong GetULong(this XElement elem, string tagName)
        {
            return elem.GetULong(tagName, default(ulong));
        }

        public static ulong GetULong(this XElement elem, string tagName, ulong defaultValue /* = 0*/)
        {
            ulong result;
            var val = elem.TagValue(tagName);

            return val == string.Empty ||
                !ulong.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static double GetDouble(this XElement elem, string tagName)
        {
            return elem.GetDouble(tagName, default(double));
        }

        public static double GetDouble(this XElement elem, string tagName, double defaultValue /* = 0*/)
        {
            double result;
            var val = elem.TagValue(tagName);

            return String.IsNullOrEmpty(val) ||
                !double.TryParse(val, out result)
                    ? defaultValue
                    : result;
        }

        public static string GetString(this XElement elem, string tagName)
        {
            return elem.GetString(tagName, String.Empty);
        }

        public static string GetString(this XElement elem, string tagName, string defaultValue /* = ""*/)
        {
            var val = elem.TagValue(tagName);

            return val == null
                    ? defaultValue
                    : val;
        }

        public static DateTime GetDate(this XElement elem, string tagName)
        {
            return elem.GetDate(tagName, DateTime.MinValue);
        }

        public static DateTime GetDate(this XElement elem, string tagName, DateTime defaultValue)
        {
            DateTime result;
            var val = elem.TagValue(tagName);

            return String.IsNullOrEmpty(val) ||
                !DateTime.TryParseExact(val,
                         "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result)
                    ? defaultValue
                    : result;
        }
    }
}
