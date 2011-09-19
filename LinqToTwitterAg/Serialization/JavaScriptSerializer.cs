using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LinqToTwitter.Json
{
    public class JavaScriptSerializer
    {
        IEnumerable<JavaScriptConverter> converters;

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            this.converters = converters;
        }

        public T ConvertToType<T>(object obj)
        {
            Type deserializedType = typeof(T);

            var converter = GetConverter(deserializedType);

            object deserializedInstance = 
                converter.Deserialize(obj as IDictionary<string, object>, deserializedType, this);

            return (T)deserializedInstance;
        }

        public T Deserialize<T>(string input)
        {
            if (converters == null)
            {
                throw new InvalidOperationException("You must assign a converter first.");
            }

            Type deserializedType = typeof(T);
            bool isArray = deserializedType.IsArray;

            if (isArray)
            {
                deserializedType = deserializedType.GetElementType();
            }

            var converter = GetConverter(deserializedType);

            if (converter == null)
            {
                throw new InvalidOperationException(
                    "You must assign a converter for type " + deserializedType.ToString() + ".");
            }

            XDocument xDoc = null;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
                xDoc = XDocument.Load(reader);
            }

            if (isArray)
            {
                var instances =
                    (from elem in xDoc.Root.Elements()
                     let dict = BuildDictionaryFromXml(elem)
                     select converter.Deserialize(dict, deserializedType, this))
                    .ToArray();

                var returnArray = Array.CreateInstance(deserializedType, instances.Length);

                for (int i = 0; i < instances.Length; i++)
                {
                    returnArray.SetValue(instances[i], i);
                }

                return (T)(returnArray as object);
            }
            else
            {
                IDictionary<string, object> dict = BuildDictionaryFromXml(xDoc.Root);

                object instance = converter.Deserialize(dict, deserializedType, this);

                return (T)instance; 
            }
        }
  
        private JavaScriptConverter GetConverter(Type deserializedType)
        {
            var converter =
                (from conv in this.converters
                 where conv.SupportedTypes.Contains(deserializedType)
                 select conv)
                .FirstOrDefault();

            return converter;
        }

        const string ArrayType = "array";

        private IDictionary<string, object> BuildDictionaryFromXml(XElement xElem)
        {
            if (!xElem.HasElements)
            {
                return null;
            }
            else if (xElem.Attribute("type") != null && xElem.Attribute("type").Value == ArrayType)
            {
                List<object> value = BuildList(xElem);
                var item = xElem.Attribute("item");

                string key = item == null ? xElem.Name.ToString() : item.Value;

                return new Dictionary<string, object> { { key, value } };
            }
            else
            {
                return
                    (from elem in xElem.Elements()
                     select new
                     {
                         Dictionary = BuildDictionaryFromXml(elem),
                         Element = elem,
                         Type = elem.Attribute("type")
                     })
                    .ToDictionary(
                        key =>
                        {
                            if (key.Type != null && key.Type.Value == ArrayType)
                            {
                                return key.Element.Attribute("item").Value;
                            }
                            else
                            {
                                return key.Element.Name.ToString();
                            }
                        },
                        val =>
                        {
                            if (val.Type != null && val.Type.Value == ArrayType)
                            {
                                string item = val.Element.Attribute("item").Value;
                                return val.Dictionary[item];
                            }
                            else
                            {
                                return val.Dictionary == null ? ParseValue(val.Element.Value) : val.Dictionary as object;
                            }
                        });
            }
        }

        private List<object> BuildList(XElement xElem)
        {
            var elements =
                (from elem in xElem.Elements()
                 select BuildDictionaryFromXml(elem) as object)
                .ToList();

            return elements;
        }

        private object ParseValue(string val)
        {
            int intVal = 0;
            double doubleVal = 0;

            if (int.TryParse(val, out intVal))
            {
                return intVal;
            }
            else if (double.TryParse(val, out doubleVal))
            {
                return doubleVal;
            }
            else
	        {
                return val;
	        }
        }
    }
}
