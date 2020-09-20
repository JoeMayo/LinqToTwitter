using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Annotation
    {
        public Annotation() { }
        public Annotation(JsonElement annotationJson)
        {
            if (annotationJson.IsNull())
            {
                Attributes = new Dictionary<string, string>();
                Elements = new Dictionary<string, string>();
                return;
            }

            Attributes =
                annotationJson
                    .EnumerateObject()
                    .ToDictionary(
                        atr => atr.Name,
                        atr => atr.Value.GetString() ?? string.Empty);
        }

        public string? Type { get; set; }

        [XmlIgnore]
        public Dictionary<string, string>? Attributes { get; set; }

        [XmlIgnore]
        public Dictionary<string, string>? Elements { get; set; }
    }
}
