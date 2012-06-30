using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using LitJson;

namespace LinqToTwitter
{
    public class Annotation
    {
        public Annotation() { }
        public Annotation(JsonData annotationJson)
        {
            if (annotationJson == null)
            {
                Attributes = new Dictionary<string, string>();
                return;
            }

            var attrDictionary = annotationJson as IDictionary<string, JsonData>;
            Attributes =
                (from string key in attrDictionary.Keys
                 select new
                 {
                     Key = key,
                     Value = attrDictionary[key]
                 })
                .ToDictionary(
                    atr => atr.Key.ToString(),
                    atr => atr.Value.ToString());
        }

        public string Type { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Attributes { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Elements { get; set; }
    }
}
