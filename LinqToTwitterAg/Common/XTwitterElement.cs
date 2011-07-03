using System;
using System.Globalization;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public static class XTwitterElement
    {
        private static string TagValue(this XElement elem, XName name)
        {
            if (elem == null)
                return null;

            var val = elem.Element(name);
            return val == null ? null : val.Value;
        }

        public static bool GetBool(this XElement elem, XName name)
        {
            return elem.GetBool(name, default(bool));
        }

        public static bool GetBool(this XElement elem, XName name, bool defaultValue /* = false*/)
        {
            bool result;
            var val = elem.TagValue(name);

            return string.IsNullOrEmpty(val) ||
                !bool.TryParse(val, out result)
                    ? defaultValue
                    : bool.Parse(elem.Element(name).Value);
        }

        public static int GetInt(this XElement elem, XName name)
        {
            return elem.GetInt(name, default(int));
        }

        public static int GetInt(this XElement elem, XName name, int defaultValue /*= 0*/)
        {
            int result;
            var val = elem.TagValue(name);

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
            ulong result;
            var val = elem.TagValue(name);

            return val == string.Empty ||
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
            double result;
            var val = elem.TagValue(name);

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
            DateTime result;
            var val = elem.TagValue(name);

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
