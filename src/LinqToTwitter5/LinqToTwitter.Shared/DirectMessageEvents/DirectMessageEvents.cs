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

        ///// <summary>
        ///// Input (New DM): ID of the user the DM is sent to
        ///// </summary>
        //public ulong RecipientID { get; set; }

        ///// <summary>
        ///// Input (New DM): DM contents
        ///// </summary>
        //public string Text { get; set; }

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

        /// <summary>
        /// ID for the next page or null if there isn't a next page
        /// </summary>
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
        /// Twitter entity before this DM
        /// </summary>
        [JsonProperty("initiated_via")]
        public InitiatedVia InitiatedVia { get; set; }

        /// <summary>
        /// Twitter container for message and metadata
        /// </summary>
        [JsonProperty("message_create")]
        public DirectMessageCreate MessageCreate { get; set; }

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

    public class InitiatedVia
    {
        [JsonProperty("tweet_id")]
        public string TweetId { get; set; }
        [JsonProperty("welcome_message_id")]
        public string WelcomeMessageId { get; set; }
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

        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }

        /// <summary>
        /// Extracted entities and indices in Text where they occur
        /// </summary>
        [JsonProperty("entities")]
        public Entities Entities { get; set; }

        [JsonProperty("quick_reply")]
        public QuickReply QuickReply { get; set; }

        [JsonProperty("ctas")]
        public IEnumerable<CallToAction> CallToActions { get; set; }
    }

    public class Attachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("location")]
        public DirectMessageEventLocation Location { get; set; }
        [JsonProperty("media")]
        public DirectMessageMedia Media { get; set; }
    }

    public class DirectMessageEventLocation
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("shared_coordinate")]
        public SharedCoordinate SharedCoordinate { get; set; }
        [JsonProperty("shared_place")]
        public SharedPlace SharedPlace { get; set; }
    }

    public class SharedCoordinate
    {
        [JsonProperty("coordinates")]
        public DirectMessageEventCoordinates Coordinates { get; set; }
    }

    public class DirectMessageEventCoordinates
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }

    public class SharedPlace
    {
        [JsonProperty("place")]
        public DirectMessageEventPlace Place { get; set; }
    }

    public class DirectMessageEventPlace
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class DirectMessageMedia
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("id_str")]
        public string IdStr { get; set; }
        [JsonProperty("indices")]
        public int[] Indices { get; set; }
        [JsonProperty("media_url")]
        public string MediaUrl { get; set; }
        [JsonProperty("media_url_https")]
        public string MediaUrlHttps { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }
        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("sizes")]
        public Sizes Sizes { get; set; }
    }

    public class Sizes
    {
        [JsonProperty("small")]
        public MediaSize Small { get; set; }
        [JsonProperty("medium")]
        public MediaSize Medium { get; set; }
        [JsonProperty("large")]
        public MediaSize Large { get; set; }
        [JsonProperty("thumb")]
        public MediaSize Thumb { get; set; }
    }

    public class MediaSize
    {
        [JsonProperty("w")]
        public int Width { get; set; }
        [JsonProperty("h")]
        public int Height { get; set; }
        [JsonProperty("resize")]
        public string Resize { get; set; }
    }

    public class QuickReply
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("options")]
        public IEnumerable<QuickReplyOption> Options { get; set; }

    }

    public class QuickReplyOption
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("metadata")]
        public string Metadata { get; set; }
    }

    public class CallToAction
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("tco_url")]
        public string TcoUrl { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
