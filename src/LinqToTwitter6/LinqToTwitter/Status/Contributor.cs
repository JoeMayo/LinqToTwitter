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
            ID = contributorJson.GetProperty("id_str").GetString();
            ScreenName = contributorJson.GetProperty("screen_name").GetString();
        }

        public string ID { get; set; }

        public string ScreenName { get; set; }
    }
}
