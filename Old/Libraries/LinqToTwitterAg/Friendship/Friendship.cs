using System;
using System.Collections.Generic;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// parameters for freindship actions
    /// </summary>
    public class Friendship
    {
        public const ulong UserIDNotIncluded = 0;
        public const string ScreenNameNotIncluded = null;

        public Friendship() { }
        public Friendship(JsonData friendJson)
        {
            if (friendJson == null) return;

            TargetRelationship = new Relationship(friendJson.GetValue<JsonData>("target"));
            SourceRelationship = new Relationship(friendJson.GetValue<JsonData>("source"));
        }

        /// <summary>
        /// type of friendship (defaults to Exists)
        /// </summary>
        public FriendshipType Type { get; set; }

        /// <summary>
        /// ID of source user (Show query)
        /// </summary>
        public string SourceUserID { get; set; }

        /// <summary>
        /// Screen name of source user (Show query)
        /// </summary>
        public string SourceScreenName { get; set; }

        /// <summary>
        /// ID of target user (Show query)
        /// </summary>
        public string TargetUserID { get; set; }

        /// <summary>
        /// Screen name of target user (Show query)
        /// </summary>
        public string TargetScreenName { get; set; }

        /// <summary>
        /// Number of items to get for FriendshipList and FollowerList queries (input)
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Helps in paging results for queries such as incoming and outgoing
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Comma-separated list of screen names for Lookup query
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of user IDs for Lookup query
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Removes status when set to true (false by default)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        [Obsolete("Please use IncludeUserEntities instead.")]
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Removes entities on users when set to false (true by default)
        /// </summary>
        public bool IncludeUserEntities { get; set; }

        /// <summary>
        /// info on friend
        /// </summary>
        public User Friend { get; set; }

        /// <summary>
        /// Relationship details returned from Twitter for the source (Show query)
        /// </summary>
        public Relationship SourceRelationship { get; set; }

        /// <summary>
        /// Relationship details returned from Twitter for the target (Show query)
        /// </summary>
        public Relationship TargetRelationship { get; set; }

        /// <summary>
        /// List of ids returned by Incoming and Outgoing queries
        /// </summary>
        public IDList IDInfo { get; set; }

        /// <summary>
        /// List of relationships from Lookup query
        /// </summary>
        public List<Relationship> Relationships { get; set; }

        /// <summary>
        /// Holds prev/next cursors
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// List of User that are friends or followers, depending on type of query
        /// </summary>
        public List<User> Users { get; set; }
    }
}
