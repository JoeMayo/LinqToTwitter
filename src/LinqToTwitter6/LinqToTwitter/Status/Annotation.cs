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

            // TODO: re-write as JsonElement
            //var attrDictionary = annotationJson as IDictionary<string, JsonData>;
            //Attributes =
            //    (from string key in attrDictionary.Keys
            //     select new
            //     {
            //         Key = key,
            //         Value = attrDictionary[key]
            //     })
            //    .ToDictionary(
            //        atr => atr.Key.ToString(),
            //        atr => atr.Value.ToString());
        }

        public string Type { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Attributes { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Elements { get; set; }
    }
}
