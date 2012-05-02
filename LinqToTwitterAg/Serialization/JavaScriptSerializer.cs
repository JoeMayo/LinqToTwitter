//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;

//using LitJson;

//namespace LinqToTwitter.Json
//{
//    public class JavaScriptSerializer
//    {
//        IEnumerable<JavaScriptConverter> converters;
//        Type deserializedType;

//        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
//        {
//            this.converters = converters;
//        }

//        public T ConvertToType<T>(object obj)
//        {
//            deserializedType = typeof(T);

//            var converter = GetConverter(deserializedType);

//            object deserializedInstance = 
//                converter.Deserialize(obj as IDictionary<string, object>, deserializedType, this);

//            return (T)deserializedInstance;
//        }

//        public T Deserialize<T>(string input)
//        {
//            if (converters == null)
//            {
//                throw new InvalidOperationException("You must assign a converter first.");
//            }

//            deserializedType = typeof(T);
//            bool isArray = deserializedType.IsArray;

//            if (isArray)
//            {
//                deserializedType = deserializedType.GetElementType();
//            }

//            var converter = GetConverter(deserializedType);

//            if (converter == null)
//            {
//                throw new InvalidOperationException(
//                    "You must assign a converter for type " + deserializedType.ToString() + ".");
//            }

//            var reader = new JsonReader(input);
//            JsonData data = JsonMapper.ToObject(reader);

//            if (isArray)
//            {
//                var instances =
//                (from JsonData json in data
//                 let dict = BuildDictionaryFromJson(null, json)
//                 select converter.Deserialize(dict, deserializedType, this))
//                    .ToArray();

//                var returnArray = Array.CreateInstance(deserializedType, instances.Length);

//                for (int i = 0; i < instances.Length; i++)
//                {
//                    returnArray.SetValue(instances[i], i);
//                }

//                return (T)(returnArray as object);
//            }
//            else
//            {
//                IDictionary<string, object> dict = BuildDictionaryFromJson(null, data);

//                object instance = converter.Deserialize(dict, deserializedType, this);

//                return (T)instance;
//            }
//        }
 
//        private JavaScriptConverter GetConverter(Type deserializedType)
//        {
//            var converter =
//                (from conv in this.converters
//                 where conv.SupportedTypes.Contains(deserializedType)
//                 select conv)
//                .FirstOrDefault();

//            return converter;
//        }

//        private IDictionary<string, object> BuildDictionaryFromJson(string inKey, JsonData data)
//        {
//            if (data == null || !(data.IsArray || data.IsObject))
//            {
//                return null;
//            }
//            else if (data.IsArray)
//            {
//                List<object> value = BuildList(data);

//                return new Dictionary<string, object> { { inKey, value } };
//            }
//            else
//            {
//                return
//                    (from string key in (data as IOrderedDictionary).Keys
//                     select new
//                     {
//                         Key = key,
//                         Dictionary = BuildDictionaryFromJson(key, data[key]),
//                         Element = data[key] ?? string.Empty
//                     })
//                    .ToDictionary(
//                        key =>
//                        {
//                            return key.Key;
//                        },
//                        val =>
//                        {
//                            if (val.Element.IsArray)
//                            {
//                                return val.Dictionary[val.Key];
//                            }

//                            return val.Dictionary == null ? 
//                                ParseValue(inKey, val.Key, val.Element.ToString()) : 
//                                val.Dictionary as object;
//                        });
//            }
//        }

//        private List<object> BuildList(JsonData data)
//        {
//            List<object> elements;

//            if (data.IsArray)
//            {
//                elements =
//                    (from object dat in data
//                     select dat)
//                    .ToList();
//            }
//            else
//            {
//                elements =
//                    (from string key in (data as IOrderedDictionary).Keys
//                     select BuildDictionaryFromJson(key, data[key]) as object)
//                    .ToList();
//            }

//            return elements;
//        }

//        object ParseValue(string inKey, string key, string val)
//        {
//            var propertyType = GetPropertyType(inKey, key);

//            switch (propertyType)
//            {
//                case "Boolean":
//                    bool boolVal;
//                    bool.TryParse(val, out boolVal);
//                    return boolVal;
//                case "Int32":
//                    int intVal;
//                    int.TryParse(val, out intVal);
//                    return intVal;
//                case "Int64":
//                    long longVal;
//                    long.TryParse(val, out longVal);
//                    return longVal;
//                case "UInt32":
//                    uint uintVal;
//                    uint.TryParse(val, out uintVal);
//                    return uintVal;
//                case "UInt64":
//                    ulong ulongVal;
//                    ulong.TryParse(val, out ulongVal);
//                    return ulongVal;
//                case "Single":
//                    float floatVal;
//                    float.TryParse(val, out floatVal);
//                    return floatVal;
//                case "Double":
//                    double doubleVal;
//                    double.TryParse(val, out doubleVal);
//                    return doubleVal;
//                case "Decimal":
//                    decimal decimalVal;
//                    decimal.TryParse(val, out decimalVal);
//                    return decimalVal;
//                case "String":
//                    return val.ToString(CultureInfo.InvariantCulture);
//                default:
//                    return val;
//            }
//        }

//        readonly Dictionary<string, Type> typeTable =
//            new Dictionary<string, Type>
//            {
//                {"status", typeof (Status)},
//                {"sleep_time", typeof(SleepTime)},
//                {"time_zone", typeof(TimeZone)},
//                {"trend_location", typeof(Place)}
//            };

//        string GetPropertyType(string inKey, string key)
//        {
//            string typeKey = inKey == null ? null : inKey.ToLower();
//            Type currentType =
//                (typeKey == null || !typeTable.ContainsKey(typeKey)) ? deserializedType : typeTable[typeKey];

//            PropertyInfo propInfo = currentType.GetProperty(key);
//            string propertyType =
//                propInfo == null ? "Unknown" : propInfo.PropertyType.Name;
//            return propertyType;
//        }
//    }
//}
