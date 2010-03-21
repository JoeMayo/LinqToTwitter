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
        /// FollowingUser is following SubjectUser
        /// </summary>
        public bool IsFriend { get; set; }

        /// <summary>
        /// info on friend
        /// </summary>
        public User Friend { get; set; }
    }
}
