using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class StatusMetaData
    {
        public StatusMetaData() { }
        public StatusMetaData(JsonElement mdJson)
        {
            ResultType = mdJson.GetProperty("result_type").GetString();
            IsoLanguageCode = mdJson.GetProperty("iso_language_code").GetString();
        }

        public string ResultType { get; set; }

        public string IsoLanguageCode { get; set; }
    }
}
