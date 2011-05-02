using System;
using System.Globalization;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public static class XTwitterElement
    {
        public static bool GetBool(this XElement elem, string tagName, bool defaultValue = false)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : bool.Parse(elem.Element(tagName).Value);
        }

        public static int GetInt(this XElement elem, string tagName, int defaultValue = 0)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : int.Parse(elem.Element(tagName).Value);
        }

        public static ulong GetULong(this XElement elem, string tagName, ulong defaultValue = 0)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : ulong.Parse(elem.Element(tagName).Value);
        }

        public static double GetDouble(this XElement elem, string tagName, double defaultValue = 0)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : double.Parse(elem.Element(tagName).Value, CultureInfo.InvariantCulture);
        }

        public static string GetString(this XElement elem, string tagName, string defaultValue = "")
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : elem.Element(tagName).Value;
        }

        public static DateTime GetDate(this XElement elem, string tagName)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? DateTime.MinValue
                    : DateTime.ParseExact(
                        elem.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        public static DateTime GetDate(this XElement elem, string tagName, DateTime defaultValue)
        {
            return
                elem.Element(tagName) == null ||
                elem.Element(tagName).Value == string.Empty
                    ? defaultValue
                    : DateTime.ParseExact(
                        elem.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

    }
}
