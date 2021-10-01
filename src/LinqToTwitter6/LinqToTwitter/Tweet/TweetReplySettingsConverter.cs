using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class TweetReplySettingsConverter : JsonConverter<TweetReplySettings>
    {
        public override TweetReplySettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() switch
            {
                "everyone" => TweetReplySettings.Everyone,
                "following" => TweetReplySettings.Following,
                "mentionedUsers" => TweetReplySettings.MentionedUsers,
                _ => TweetReplySettings.None
            };
        }

        public override void Write(Utf8JsonWriter writer, TweetReplySettings value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value switch
                {
                    TweetReplySettings.Everyone => "everyone",
                    TweetReplySettings.Following => "following",
                    TweetReplySettings.MentionedUsers => "mentionedUsers",
                    _ => ""
                });
        }
    }
}