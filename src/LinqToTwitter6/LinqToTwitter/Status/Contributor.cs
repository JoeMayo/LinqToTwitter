using LinqToTwitter.Common;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Contributor
    {
        public Contributor() { }
        public Contributor(JsonElement contributorJson)
        {
            ID = contributorJson.GetString("id_str");
            ScreenName = contributorJson.GetString("screen_name");
        }

        public string? ID { get; set; }

        public string? ScreenName { get; set; }
    }
}
