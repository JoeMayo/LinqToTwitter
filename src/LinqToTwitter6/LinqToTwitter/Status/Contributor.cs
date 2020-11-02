using LinqToTwitter.Common;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Contributor
    {
        public Contributor() { }
        public Contributor(JsonElement contributor)
        {
            if (contributor.IsNull()) return;

            if (contributor.ValueKind == JsonValueKind.Number)
            {
                ID = contributor.GetInt64().ToString();
            }
            else
            {
                ID = contributor.GetLong("id_str").ToString();
                ScreenName = contributor.GetString("screen_name"); 
            }
        }

        public string? ID { get; set; }

        public string? ScreenName { get; set; }
    }
}
