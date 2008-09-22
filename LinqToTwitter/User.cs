/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public class User
    {
        /// <summary>
        /// user's Twitter ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// name of user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// user's screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// location of user
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// user's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// user's image
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// user's URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// is user protected
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// number of people following user
        /// </summary>
        public int FollowersCount { get; set; }
    }
}
