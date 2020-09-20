using System.Xml.Serialization;
using System.Text.Json;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class StatusMetaData
    {
        public StatusMetaData() { }
        public StatusMetaData(JsonElement mdJson)
        {
            if (mdJson.IsNull())
                return;

            ResultType = mdJson.GetString("result_type");
            IsoLanguageCode = mdJson.GetString("iso_language_code");
        }

        public string? ResultType { get; set; }

        public string? IsoLanguageCode { get; set; }
    }
}
