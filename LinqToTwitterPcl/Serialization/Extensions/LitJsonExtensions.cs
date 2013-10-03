using System;
using LitJson;

namespace LinqToTwitter.Serialization.Extensions
{
    public static class LitJsonExtensions
    {
        public static void WriteJsonData(this JsonWriter writer, JsonData jsonData)
        {
            var reader = new JsonReader(jsonData.ToJson());

            while (reader.Read())
            {
                switch (reader.Token)
                {
                    case JsonToken.None:
                        break;
                    case JsonToken.ObjectStart:
                        writer.WriteObjectStart();
                        break;
                    case JsonToken.PropertyName:
                        writer.WritePropertyName(reader.Value.ToString());
                        break;
                    case JsonToken.ObjectEnd:
                        writer.WriteObjectEnd();
                        break;
                    case JsonToken.ArrayStart:
                        writer.WriteArrayStart();
                        break;
                    case JsonToken.ArrayEnd:
                        writer.WriteArrayEnd();
                        break;
                    case JsonToken.Int:
                        writer.Write((int)reader.Value);
                        break;
                    case JsonToken.Long:
                        writer.Write((long)reader.Value);
                        break;
                    case JsonToken.ULong:
                        writer.Write((ulong)reader.Value);
                        break;
                    case JsonToken.Double:
                        writer.Write((double)reader.Value);
                        break;
                    case JsonToken.String:
                        writer.Write((string)reader.Value);
                        break;
                    case JsonToken.Boolean:
                        writer.Write((bool)reader.Value);
                        break;
                    case JsonToken.Null:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
