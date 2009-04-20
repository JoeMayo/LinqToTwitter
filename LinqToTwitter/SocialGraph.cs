using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// social graph information
    /// </summary>
    public class SocialGraph
    {
        /// <summary>
        /// type of request
        /// </summary>
        public SocialGraphType Type { get; set; }

        /// <summary>
        /// The ID or screen_name of the user to retrieve the friends ID list for
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Specfies the ID of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid user ID is also a valid screen name. 
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Specfies the screen name of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid screen name is also a user ID.
        /// </summary>
        public string ScreenName { get; set; }
    }
}
