using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Some queries return only one list, rather than an array of lists.
    /// This ensures we deserialize to a List<<see cref="List"/>> regardless.
    /// </summary>
    public class ListConverter : JsonConverter<List<List>>
    {
        public override List<List>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            string? input = jsonDoc.RootElement.GetRawText();

            if (reader.TokenType == JsonTokenType.StartArray || 
                reader.TokenType == JsonTokenType.EndArray) // is array of lists
            {
                return JsonSerializer.Deserialize<List<List>>(input);
            }
            else // is single list
            {
                List? list = JsonSerializer.Deserialize<List>(input);
                return list == null ? null : (new() { list });
            }
        }

        public override void Write(Utf8JsonWriter writer, List<List> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(JsonSerializer.Serialize(value));
        }
    }
}
