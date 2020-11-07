using System.Linq;
using System.Collections.Generic;
using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// Friendship details for either a Source or Target
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Relationship
    {
        public Relationship() { }
        public Relationship(JsonElement relJson)
        {
            if (relJson.IsNull()) return;

            ScreenName = relJson.GetString("screen_name");
            Name = relJson.GetString("name");
            RetweetsWanted = relJson.GetBool("want_retweets");
            AllReplies = relJson.GetBool("all_replies");
            MarkedSpam = relJson.GetBool("marked_spam");
            ID = relJson.GetUlong("id");
            Blocking = relJson.GetBool("blocking");
            NotificationsEnabled = relJson.GetBool("notifications_enabled");
            CanDm = relJson.GetBool("can_dm");
            Muting = relJson.GetBool("muting", false);

            var connections = relJson.GetProperty("connections");
            if (connections.IsNull())
                Connections =
                    (from connection in connections.EnumerateArray()
                     select connection.ToString())
                    .ToList();
            else
                Connections = new List<string>();

            FollowedBy = 
                relJson.GetBool("followed_by") ||
                Connections.Contains("followed_by");
            Following = 
                relJson.GetBool("following") ||
                Connections.Contains("following");
        }

        /// <summary>
        /// User ID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// User's screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is this user following the other
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// Does the other user follow this one
        /// </summary>
        public bool FollowedBy { get; set; }

        /// <summary>
        /// Is this user blocking the other
        /// (null means that Twitter doesn't provide the value for privacy reasons)
        /// </summary>
        public bool? Blocking { get; set; }

        /// <summary>
        /// Are this user's notifications enabled
        /// (null means that Twitter doesn't provide the value for privacy reasons)
        /// </summary>
        public bool? NotificationsEnabled { get; set; }

        /// <summary>
        /// Does the user want to receive retweets from person they follow
        /// </summary>
        public bool RetweetsWanted { get; set; }

        /// <summary>
        /// Shows relationships between the logged in user and 
        /// the person identified by this relationship
        /// </summary>
        public List<string> Connections { get; set; }

        /// <summary>
        /// Sees all replies
        /// </summary>
        public bool AllReplies { get; set; }

        /// <summary>
        /// Marked as SPAM
        /// </summary>
        public bool MarkedSpam { get; set; }

        /// <summary>
        /// Allowed to send direct messages
        /// </summary>
        public bool CanDm { get; set; }

        /// <summary>
        /// User is muted.
        /// </summary>
        public bool Muting { get; set; }
    }
}
