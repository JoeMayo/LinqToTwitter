using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// parameters for freindship actions
    /// </summary>
    [Serializable]
    public class Friendship
    {
        /// <summary>
        /// type of friendship (defaults to Exists)
        /// </summary>
        public FriendshipType Type { get; set; }

        /// <summary>
        /// The ID or screen_name of the subject user
        /// </summary>
        public string SubjectUser { get; set; }

        /// <summary>
        /// The ID or screen_name of the user to test for following
        /// </summary>
        public string FollowingUser { get; set; }

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
        /// Helps in paging results for queries such as incoming and outgoing
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// FollowingUser is following SubjectUser returned from Twitter (Exists query)
        /// </summary>
        public bool IsFriend { get; set; }

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

        public IDList IDInfo { get; set; }
    }
}
