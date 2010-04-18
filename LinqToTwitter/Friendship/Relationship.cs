using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Friendship details for either a Source or Target
    /// </summary>
    [Serializable]
    public class Relationship
    {
        /// <summary>
        /// Creates a new relationship object from XML info
        /// </summary>
        /// <param name="relationship">XML with info</param>
        /// <returns>Relationship instance</returns>
        public Relationship CreateRelationship(XElement relationship)
        {
            if (relationship == null || relationship.Value == null)
            {
                return null;
            }

            return new Relationship
            {
                ID = relationship.Element("id").Value,
                ScreenName = relationship.Element("screen_name").Value,
                Following = bool.Parse(relationship.Element("following").Value),
                FollowedBy = bool.Parse(relationship.Element("followed_by").Value),
                Blocking =
                    relationship.Element("blocking") == null ||
                    relationship.Element("blocking").Value == string.Empty ?
                        (bool?)null :
                        bool.Parse(relationship.Element("blocking").Value),
                NotificationsEnabled =
                    relationship.Element("notifications_enabled") == null ||
                    relationship.Element("notifications_enabled").Value == string.Empty ?
                        (bool?)null :
                        bool.Parse(relationship.Element("notifications_enabled").Value)
            };
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
    }
}
