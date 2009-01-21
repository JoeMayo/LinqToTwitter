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
    /// <summary>
    /// information for a twitter user
    /// </summary>
    public class User
    {
        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// user's Twitter ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// used in search for user by email
        /// </summary>
        public string Email { get; set; }

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

        /// <summary>
        /// color of profile background
        /// </summary>
        public string ProfileBackgroundColor { get; set; }

        /// <summary>
        /// color of profile text
        /// </summary>
        public string ProfileTextColor { get; set; }

        /// <summary>
        /// color of profile links
        /// </summary>
        public string ProfileLinkColor { get; set; }

        /// <summary>
        /// color of profile sidebar
        /// </summary>
        public string ProfileSidebarFillColor { get; set; }

        /// <summary>
        /// color of profile sidebar border
        /// </summary>
        public string ProfileSidebarBorderColor { get; set; }

        /// <summary>
        /// number of friends
        /// </summary>
        public int FriendsCount { get; set; }

        /// <summary>
        /// date and time when profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// number of favorites
        /// </summary>
        public int FavoritesCount { get; set; }

        /// <summary>
        /// UTC Offset
        /// </summary>
        public string UtcOffset { get; set; }

        /// <summary>
        /// Time Zone
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// URL of profile background image
        /// </summary>
        public string ProfileBackgroundImageUrl { get; set; }

        /// <summary>
        /// Title of profile background
        /// </summary>
        public string ProfileBackgroundTile { get; set; }

        /// <summary>
        /// number of status updates user has made
        /// </summary>
        public int StatusesCount { get; set; }

        /// <summary>
        /// current user status (valid only in user queries)
        /// </summary>
        public Status Status { get; set; }
    }
}
