using System.Xml.Serialization;
using Newtonsoft.Json;
using System;
using LinqToTwitter.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// Direct message events support Twitter chatbot messages.
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class WelcomeMessage
    {
        /// <summary>
        /// Input (New Welcome Message): Name of the Welcome Message
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Input (New Welcome Message): Welcome Message contents
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Input (All Queries): Type of Welcome Message
        /// </summary>
        public WelcomeMessageType Type { get; set; }

        /// <summary>
        /// Output: Response from Twitter
        /// </summary>
        public WelcomeMessageValue Value { get; set; }
    }

    public class WelcomeMessageValue
    {
        [JsonProperty("apps")]
        public JObject Apps { get; set; }
        [JsonProperty("welcome_message")]
        public WelcomeMsg WelcomeMessage { get; set; }
    }

    public class WelcomeMsg
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }
        [JsonProperty("source_app_id")]
        public string SourceAppId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("message_data")]
        public WelcomeMessageData MessageData { get; set; }


        /// <summary>
        /// Helper property for C# DateTime matching CreatedTimestamp (so you don't have to convert it yourself)
        /// </summary>
        DateTime createdAt;
        public DateTime CreatedAt
        {
            get
            {
                if (createdAt == default(DateTime))
                    createdAt = CreatedTimestamp.GetEpochDateFromTimestamp();

                return createdAt;
            }
        }
    }

    public class WelcomeMessageData
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("entities")]
        public Entities Entities { get; set; }
        [JsonProperty("attachment")]
        public WelcomeMessageAttachment Attachment { get; set; }
    }

    public class WelcomeMessageAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("media")]
        public WelcomeMessageMedia Media { get; set; }
    }

    public class WelcomeMessageMedia
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
