using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using LitJson;

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

            var reader = new JsonReader(input);
            JsonData data = JsonMapper.ToObject(reader);

            if (isArray)
            {
                var instances =
                (from JsonData json in data
                 let dict = BuildDictionaryFromJson(null, json)
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
                IDictionary<string, object> dict = BuildDictionaryFromJson(null, data);

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

        private IDictionary<string, object> BuildDictionaryFromJson(string inKey, JsonData data)
        {
            if (data == null || !(data.IsArray || data.IsObject))
            {
                return null;
            }
            else if (data.IsArray)
            {
                List<object> value = BuildList(data);

                return new Dictionary<string, object> { { inKey, value } };
            }
            else
            {
                return
                    (from string key in (data as IOrderedDictionary).Keys
                     select new
                     {
                         Key = key,
                     Dictionary = BuildDictionaryFromJson(key, data[key]),
                     Element = data[key] ?? string.Empty
                     })
                    .ToDictionary(
                        key =>
                        {
                            return key.Key;
                        },
                        val =>
                        {
                            if (val.Element.IsArray)
                            {
                                return val.Dictionary[val.Key];
                            }
                            else
                            {
                                return val.Dictionary == null ? 
                                    ParseValue(val.Element.ToString()) : 
                                    val.Dictionary as object;
                            }
                        });
            }
        }

        private List<object> BuildList(JsonData data)
        {
            var elements =
            (from string key in (data as IOrderedDictionary).Keys
             select BuildDictionaryFromJson(key, data[key]) as object)
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
