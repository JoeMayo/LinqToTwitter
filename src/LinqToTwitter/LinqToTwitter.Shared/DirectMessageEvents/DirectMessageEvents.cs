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
    public class DirectMessageEvents
    {
        /// <summary>
        /// Input (List Query): Number of items to return for a single page.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Input (List Query): Supports paging through results on List queries
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Input (Show Query): ID of DM
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// Input (New DM): ID of the user the DM is sent to
        /// </summary>
        public ulong RecipientID { get; set; }

        /// <summary>
        /// Input (New DM): DM contents
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Input (All Queries): Type of Direct Message Events
        /// </summary>
        public DirectMessageEventsType Type { get; set; }

        /// <summary>
        /// Output: Response from Twitter
        /// </summary>
        public DirectMessageEventsValue Value { get; set; }
    }

    public class DirectMessageEventsValue
    {
        /// <summary>
        /// Twitter DM event container for a single event. e.g. Show query or NewDirectMessageEventAsync call
        /// </summary>
        [JsonProperty("event")]
        public DMEvent DMEvent { get; set; }

        /// <summary>
        /// Twitter DM event container for multiple events e.g. List query
        /// </summary>
        [JsonProperty("events")]
        public List<DMEvent> DMEvents { get; set; }

        /// <summary>
        /// Show and List queries populate this to show which app created the DM.
        /// You need to use JSON.NET because the nested object ID is a property matching the app id, 
        /// which is different for every app, precluding the ability to assign a C# property.
        /// </summary>
        [JsonProperty("apps")]
        public JObject Apps { get; set; }

        [JsonProperty("next_cursor")]
        public string NextCursor { get; set; }
    }

    public class DMEvent
    {
        /// <summary>
        /// Type of event: message_create for new DMs
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// DM ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Milliseconds since 1/1/1970 00:00:00 when event was created
        /// </summary>
        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }

        /// <summary>
        /// Twitter container for message and metadata
        /// </summary>
        [JsonProperty("message_create")]
        public DirectMessageCreate MessageCreate { get; set; }

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

    public class DirectMessageCreate
    {
        /// <summary>
        /// Who the DM is sent to
        /// </summary>
        [JsonProperty("target")]
        public DirectMessageTarget Target { get; set; }

        /// <summary>
        /// ID of person who sent the DM.
        /// Populated for DM Show and List queries.
        /// </summary>
        [JsonProperty("sender_id")]
        public string SenderID { get; set; }

        /// <summary>
        /// ID of the application creating the DM
        /// </summary>
        [JsonProperty("source_app_id")]
        public string SourceAppID { get; set; }

        /// <summary>
        /// DM Contents
        /// </summary>
        [JsonProperty("message_data")]
        public DirectMessageData MessageData { get; set; }
    }

    public class DirectMessageTarget
    {
        /// <summary>
        /// ID of person DM is sent to
        /// </summary>
        [JsonProperty("recipient_id")]
        public string RecipientID { get; set; }
    }

    public class DirectMessageData
    {
        /// <summary>
        /// DM contents
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Extracted entities and indices in Text where they occur
        /// </summary>
        [JsonProperty("entities")]
        public Entities Entities { get; set; }
    }
}
