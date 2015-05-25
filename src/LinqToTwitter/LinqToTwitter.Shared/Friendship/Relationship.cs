using System;
using System.Linq;
using System.Collections.Generic;
using LinqToTwitter.Common;
using LitJson;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Friendship details for either a Source or Target
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Relationship
    {
        public Relationship() { }
        public Relationship(JsonData relJson)
        {
            if (relJson == null) return;

            ScreenName = relJson.GetValue<string>("screen_name");
            Name = relJson.GetValue<string>("name");
            RetweetsWanted = relJson.GetValue<bool>("want_retweets");
            AllReplies = relJson.GetValue<bool>("all_replies");
            MarkedSpam = relJson.GetValue<bool>("marked_spam");
            ID = relJson.GetValue<ulong>("id");
            Blocking = relJson.GetValue<bool>("blocking");
            NotificationsEnabled = relJson.GetValue<bool>("notifications_enabled");
            CanDm = relJson.GetValue<bool>("can_dm");
            Muting = relJson.GetValue<bool>("muting", false);

            var connections = relJson.GetValue<JsonData>("connections");
            if (connections != null)
                Connections =
                    (from JsonData connection in connections
                     select connection.ToString())
                    .ToList();
            else
                Connections = new List<string>();

            FollowedBy = 
                relJson.GetValue<bool>("followed_by") ||
                Connections.Contains("followed_by");
            Following = 
                relJson.GetValue<bool>("following") ||
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
