using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Account Activity support for Twitter chatbot messages.
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class AccountActivity
    {
        /// <summary>
        /// Input: ID for a webhook
        /// </summary>
        public ulong WebhookID { get; set; }

        /// <summary>
        /// Input: Url for adding a webhook
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Input (All Queries): Type of Account Activity
        /// </summary>
        public AccountActivityType Type { get; set; }

        /// <summary>
        /// Output: Indicates whether a user is subscribed. Populated via a subscription command or query.
        /// </summary>
        public SubscriptionValue SubscriptionValue { get; set; }

        /// <summary>
        /// Output: Webhooks response from Twitter. Populated via a webhook command or query.
        /// </summary>
        public WebhooksValue WebhooksValue { get; set; }
    }

    public class SubscriptionValue
    {
        /// <summary>
        /// Indicates whether the authorizing user is subscribed to a Webhook.
        /// </summary>
        /// <exception cref="TwitterQueryException">
        /// Throws TwitterQueryException when an AddAccountActivitySubscriptionAsync fails.
        /// </exception>
        public bool IsSubscribed { get; set; }
    }

    public class AccountActivityValue
    {
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

    public class WebhooksValue
    {
        public Webhook[] Webhooks { get; set; }
    }

    public class Webhook
    {
        /// <summary>
        /// Webhook ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Webhook URL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Did Webhook pass CRC
        /// </summary>
        [JsonProperty("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// UTC DateTime Webhook was created
        /// </summary>
        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }
    }

}
