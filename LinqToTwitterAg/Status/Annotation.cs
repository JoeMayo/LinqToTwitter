using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

        public static Annotation CreateAnnotation(XElement annotation)
        {
            if (annotation == null || string.IsNullOrEmpty(annotation.Value))
            {
                return null;
            }

            Annotation ann;

            if (annotation.Element("type") != null)
            {
                ann = new Annotation
                {
                    Type = annotation.Element("type").Value,
                    Attributes =
                        (from attr in annotation.Element("attributes").Elements("attribute")
                         select new
                         {
                             Key = attr.Element("name").Value,
                             Value = attr.Element("value").Value
                         })
                        .ToDictionary(
                            atr => atr.Key,
                            atr => atr.Value)
                };
            }
            else
            {
                ann = new Annotation
                {
                    Type = annotation.Attribute("type").Value,
                    Elements =
                        (from elem in annotation.Descendants()
                         select new
                         {
                            Key = elem.Name.ToString(),
                            Value = elem.Value
                         })
                        .ToDictionary(
                            atr => atr.Key,
                            atr => atr.Value)
                }; 
            }

            return ann;
        }
    }
}
