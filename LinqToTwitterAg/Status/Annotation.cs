using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    public class Annotation
    {
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
