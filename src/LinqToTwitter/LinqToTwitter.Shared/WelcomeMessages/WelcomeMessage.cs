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
        ///// <summary>
        ///// Input (New Welcome Message): Name of the Welcome Message
        ///// </summary>
        //public string Name { get; set; }

        ///// <summary>
        ///// Input (New Welcome Message): Welcome Message contents
        ///// </summary>
        //public string Text { get; set; }

        /// <summary>
        /// Number of items to return
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Helps page through results greater than Count items
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Input (New Welcome Message Rule): ID of the message to set as default.
        /// </summary>
        public ulong ID { get; internal set; }

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
        /// <summary>
        /// Single Welcome Message
        /// </summary>
        [JsonProperty("welcome_message")]
        public WelcomeMsg WelcomeMessage { get; set; }

        /// <summary>
        /// Multiple Welcome Messages
        /// </summary>
        [JsonProperty("welcome_messages")]
        public List<WelcomeMsg> WelcomeMessages { get; set; }

        /// <summary>
        /// A single message rule
        /// </summary>
        [JsonProperty("welcome_message_rule")]
        public WelcomeMessageRule WelcomeMessageRule { get; set; }

        /// <summary>
        /// Multiple message rules
        /// </summary>
        [JsonProperty("welcome_message_rules")]
        public List<WelcomeMessageRule> WelcomeMessageRules { get; set; }

        /// <summary>
        /// Show and List queries populate this to show which app created the message.
        /// You need to use JSON.NET because the nested object ID is a property matching the app id, 
        /// which is different for every app, precluding the ability to assign a C# property.
        /// </summary>
        [JsonProperty("apps")]
        public JObject Apps { get; set; }

        /// <summary>
        /// ID for the next page or null if there isn't a next page
        /// </summary>
        [JsonProperty("next_cursor")]
        public string NextCursor { get; set; }
    }

    public class WelcomeMsg
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }

        /// <summary>
        /// ID of the application creating the Message
        /// </summary>
        [JsonProperty("source_app_id")]
        public string SourceAppId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("message_data")]
        public WelcomeMessageData MessageData { get; set; }

        DateTime createdAt;
        /// <summary>
        /// Helper property for C# DateTime matching CreatedTimestamp (so you don't have to convert it yourself)
        /// </summary>
        [JsonIgnore]
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

    public class WelcomeMessageRule
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }
        [JsonProperty("welcome_message_id")]
        public string WelcomeMessageID { get; set; }

        /// <summary>
        /// Helper property for C# DateTime matching CreatedTimestamp (so you don't have to convert it yourself)
        /// </summary>
        DateTime createdAt;
        [JsonIgnore]
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
}
