using System.Xml.Serialization;
using System;
using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using LinqToTwitter.Common.Entities;

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
        public string? Cursor { get; set; }

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
        public DirectMessageEventsType? Type { get; set; }

        /// <summary>
        /// Output: Response from Twitter
        /// </summary>
        public DirectMessageEventsValue? Value { get; set; }
    }

    public class DirectMessageEventsValue
    {
        /// <summary>
        /// Twitter DM event container for a single event. e.g. Show query or NewDirectMessageEventAsync call
        /// </summary>
        [JsonPropertyName("event")]
        public DMEvent? DMEvent { get; set; }

        /// <summary>
        /// Twitter DM event container for multiple events e.g. List query
        /// </summary>
        [JsonPropertyName("events")]
        public List<DMEvent>? DMEvents { get; set; }

        /// <summary>
        /// Show and List queries populate this to show which app created the DM.
        /// You need to use JSON.NET because the nested object ID is a property matching the app id, 
        /// which is different for every app, precluding the ability to assign a C# property.
        /// </summary>
        [JsonPropertyName("apps")]
        public JsonElement Apps { get; set; }

        /// <summary>
        /// ID for the next page or null if there isn't a next page
        /// </summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }
    }

    public class DMEvent
    {
        /// <summary>
        /// Type of event: message_create for new DMs
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// DM ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Milliseconds since 1/1/1970 00:00:00 when event was created
        /// </summary>
        [JsonPropertyName("created_timestamp")]
        public string? CreatedTimestamp { get; set; }

        /// <summary>
        /// Twitter entity before this DM
        /// </summary>
        [JsonPropertyName("initiated_via")]
        public InitiatedVia? InitiatedVia { get; set; }

        /// <summary>
        /// Twitter container for message and metadata
        /// </summary>
        [JsonPropertyName("message_create")]
        public DirectMessageCreate? MessageCreate { get; set; }

        DateTime? createdAt;
        /// <summary>
        /// Helper property for C# DateTime matching CreatedTimestamp (so you don't have to convert it yourself)
        /// </summary>
        [JsonIgnore]
        public DateTime? CreatedAt
        {
            get
            {
                if (createdAt == null || createdAt == default(DateTime))
                    createdAt = CreatedTimestamp?.GetEpochDateFromTimestamp();

                return createdAt;
            }
        }
    }

    public class InitiatedVia
    {
        [JsonPropertyName("tweet_id")]
        public string? TweetId { get; set; }
        [JsonPropertyName("welcome_message_id")]
        public string? WelcomeMessageId { get; set; }
    }

    public class DirectMessageCreate
    {
        /// <summary>
        /// Who the DM is sent to
        /// </summary>
        [JsonPropertyName("target")]
        public DirectMessageTarget? Target { get; set; }

        /// <summary>
        /// ID of person who sent the DM.
        /// Populated for DM Show and List queries.
        /// </summary>
        [JsonPropertyName("sender_id")]
        public string? SenderID { get; set; }

        /// <summary>
        /// ID of the application creating the DM
        /// </summary>
        [JsonPropertyName("source_app_id")]
        public string? SourceAppID { get; set; }

        /// <summary>
        /// DM Contents
        /// </summary>
        [JsonPropertyName("message_data")]
        public DirectMessageData? MessageData { get; set; }
    }

    public class DirectMessageTarget
    {
        /// <summary>
        /// ID of person DM is sent to
        /// </summary>
        [JsonPropertyName("recipient_id")]
        public string? RecipientID { get; set; }
    }

    public class DirectMessageData
    {
        /// <summary>
        /// DM contents
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("attachment")]
        public Attachment? Attachment { get; set; }

        /// <summary>
        /// Extracted entities and indices in Text where they occur
        /// </summary>
        [JsonPropertyName("entities")]
        public Entities? Entities { get; set; }

        [JsonPropertyName("quick_reply")]
        public QuickReply? QuickReply { get; set; }

        [JsonPropertyName("ctas")]
        public IEnumerable<CallToAction>? CallToActions { get; set; }
    }

    public class Attachment
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("location")]
        public DirectMessageEventLocation? Location { get; set; }
        [JsonPropertyName("media")]
        public DirectMessageMedia? Media { get; set; }
    }

    public class DirectMessageEventLocation
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("shared_coordinate")]
        public SharedCoordinate? SharedCoordinate { get; set; }
        [JsonPropertyName("shared_place")]
        public SharedPlace? SharedPlace { get; set; }
    }

    public class SharedCoordinate
    {
        [JsonPropertyName("coordinates")]
        public DirectMessageEventCoordinates? Coordinates { get; set; }
    }

    public class DirectMessageEventCoordinates
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("coordinates")]
        public double[]? Coordinates { get; set; }
    }

    public class SharedPlace
    {
        [JsonPropertyName("place")]
        public DirectMessageEventPlace? Place { get; set; }
    }

    public class DirectMessageEventPlace
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    public class DirectMessageMedia
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("id_str")]
        public string? IdStr { get; set; }
        [JsonPropertyName("indices")]
        public int[]? Indices { get; set; }
        [JsonPropertyName("media_url")]
        public string? MediaUrl { get; set; }
        [JsonPropertyName("media_url_https")]
        public string? MediaUrlHttps { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("display_url")]
        public string? DisplayUrl { get; set; }
        [JsonPropertyName("expanded_url")]
        public string? ExpandedUrl { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("sizes")]
        public Sizes? Sizes { get; set; }
    }

    public class Sizes
    {
        [JsonPropertyName("small")]
        public MediaSize? Small { get; set; }
        [JsonPropertyName("medium")]
        public MediaSize? Medium { get; set; }
        [JsonPropertyName("large")]
        public MediaSize? Large { get; set; }
        [JsonPropertyName("thumb")]
        public MediaSize? Thumb { get; set; }
    }

    public class MediaSize
    {
        [JsonPropertyName("w")]
        public int Width { get; set; }
        [JsonPropertyName("h")]
        public int Height { get; set; }
        [JsonPropertyName("resize")]
        public string? Resize { get; set; }
    }

    public class QuickReply
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("options")]
        public IEnumerable<QuickReplyOption>? Options { get; set; }

    }

    public class QuickReplyOption
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("metadata")]
        public string? Metadata { get; set; }
    }

    public class CallToAction
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("label")]
        public string? Label { get; set; }
        [JsonPropertyName("tco_url")]
        public string? TcoUrl { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
