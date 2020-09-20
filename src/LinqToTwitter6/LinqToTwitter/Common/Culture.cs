using System.Globalization;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Culture
    {
        static CultureInfo? usCulture;

        public static CultureInfo US
        {
            get
            {
                if (usCulture == null)
                    usCulture = new CultureInfo("en-US");

                return usCulture;
            }
        }
    }
}
