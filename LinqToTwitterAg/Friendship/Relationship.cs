using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Friendship details for either a Source or Target
    /// </summary>
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
            FollowedBy = relJson.GetValue<bool>("followed_by");
            ID = relJson.GetValue<string>("id_str");
            Blocking = relJson.GetValue<bool>("blocking");
            NotificationsEnabled = relJson.GetValue<bool>("notifications_enabled");
            Following = relJson.GetValue<bool>("following");
            CanDm = relJson.GetValue<bool>("can_dm");
            var connections = relJson.GetValue<JsonData>("connections");
            if (connections != null)
            {
                Connections =
                    (from JsonData connection in connections
                        select connection.ToString())
                    .ToList(); 
            }
        }

        /// <summary>
        /// Creates a new relationship object from XML info
        /// </summary>
        /// <param name="relationship">XML with info</param>
        /// <returns>Relationship instance</returns>
        public static Relationship CreateRelationship(XElement relationshipXml)
        {
            if (relationshipXml == null || relationshipXml.Value == null)
            {
                return null;
            }

            var relationship = new Relationship
            {
                ID = relationshipXml.Element("id").Value,
                ScreenName = 
                    relationshipXml.Element("screen_name") == null ?
                        string.Empty :
                        relationshipXml.Element("screen_name").Value,
                Following =
                    relationshipXml.Element("following") == null ||
                    relationshipXml.Element("following").Value == string.Empty ?
                        false :
                        bool.Parse(relationshipXml.Element("following").Value),
                FollowedBy =
                    relationshipXml.Element("followed_by") == null ||
                    relationshipXml.Element("followed_by").Value == string.Empty ?
                        false :
                        bool.Parse(relationshipXml.Element("followed_by").Value),
                Blocking =
                    relationshipXml.Element("blocking") == null ||
                    relationshipXml.Element("blocking").Value == string.Empty ?
                        (bool?)null :
                        bool.Parse(relationshipXml.Element("blocking").Value),
                NotificationsEnabled =
                    relationshipXml.Element("notifications_enabled") == null ||
                    relationshipXml.Element("notifications_enabled").Value == string.Empty ?
                        (bool?)null :
                        bool.Parse(relationshipXml.Element("notifications_enabled").Value),
                RetweetsWanted =
                    relationshipXml.Element("want_retweets") == null ||
                    relationshipXml.Element("want_retweets").Value == string.Empty ?
                        false :
                        bool.Parse(relationshipXml.Element("want_retweets").Value)
            };

            List<string> connections = null;

            if (relationshipXml.Element("connections") != null &&
                relationshipXml.Element("connections").Elements("connection") != null)
            {
                connections =
                    (from rel in relationshipXml.Element("connections").Elements("connection")
                        select rel.Value)
                    .ToList(); 
            }
            else
            {
                connections = new List<string>();
            }

            if (relationship.FollowedBy)
            {
                connections.Add("followed_by");
            }

            if (relationship.Following)
            {
                connections.Add("following");
            }

            if (connections.Contains("followed_by"))
            {
                relationship.FollowedBy = true;
            }

            if (connections.Contains("following"))
            {
                relationship.Following = true;
            }

            relationship.Connections = connections;

            return relationship;
        }

        /// <summary>
        /// User ID
        /// </summary>
        public string ID { get; set; }

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
    }
}
