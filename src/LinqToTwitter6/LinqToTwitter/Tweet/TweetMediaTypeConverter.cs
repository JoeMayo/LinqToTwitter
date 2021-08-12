using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetMediaTypeConverter : JsonConverter<TweetMediaType>
    {
        public override TweetMediaType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() switch
            {
                "animated_gif" => TweetMediaType.AnimatedGif,
                "photo" => TweetMediaType.Photo,
                "video" => TweetMediaType.Video,
                _ => TweetMediaType.None
            };
        }

        public override void Write(Utf8JsonWriter writer, TweetMediaType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value switch
                {
                    TweetMediaType.AnimatedGif => "animated_gif",
                    TweetMediaType.Photo => "photo",
                    TweetMediaType.Video => "video",
                    _ => ""
                });
        }
    }
}