using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// helps retrieve information about blocks
    /// </summary>
    [Serializable]
    public class Blocks
    {
        /// <summary>
        /// type of blocks request to perform
        /// </summary>
        public BlockingType Type { get; set; }

        /// <summary>
        /// id or screen name of user
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// page to retrieve
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// user being blocked
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Twitter response for no block on specified user
        /// </summary>
        public TwitterHashResponse NoBlock { get; set; }
    }
}
