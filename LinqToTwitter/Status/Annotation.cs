using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [Serializable]
    public class Annotation
    {
        public string Type { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Attributes { get; set; }

        public virtual Annotation CreateAnnotation(XElement annotation)
        {
            if (annotation == null)
            {
                return null;
            }
            //XElement root = annotation.Element("annotation");

            var ann = new Annotation
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

            return ann;
        }
    }
}
