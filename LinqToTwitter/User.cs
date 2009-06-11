/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// information for a twitter user
    /// </summary>
    public class User
    {
        /// <summary>
        /// creates a new user based on an XML user fragment
        /// </summary>
        /// <param name="user">XML user fragment</param>
        /// <returns>new User instance</returns>
        public User CreateUser(XElement user)
        {
            if (user == null)
            {
                return null;
            }

            var tempUserProtected = false;
            var tempFollowersCount = 0ul;
            var tempFriendsCount = 0ul;
            var tempFavoritesCount = 0ul;
            var tempStatusesCount = 0ul;
            var tempStatusTruncated = false;
            var tempStatusFavorited = false;
            var tempFollowingUsers = false;

            var canParseProtected = 
                bool.TryParse(user.Element("protected").Value, out tempUserProtected);
            
            var followersCount =
                ulong.TryParse(user.Element("followers_count").Value, out tempFollowersCount);
            
            var friendsCount =
                user.Element("friends_count") == null ? 
                    false :
                    ulong.TryParse(user.Element("friends_count").Value, out tempFriendsCount);
            
            var userDateParts =
                user.Element("created_at") == null ?
                    null :
                    user.Element("created_at").Value.Split(' ');
            
            var userCreatedAtDate =
                userDateParts == null ?
                    DateTime.MinValue :
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT",
                        userDateParts[1],
                        userDateParts[2],
                        userDateParts[5],
                        userDateParts[3]),
                        CultureInfo.InvariantCulture);
            
            var favoritesCount =
                user.Element("favourites_count") == null ? 
                    false :
                    ulong.TryParse(user.Element("favourites_count").Value, out tempFavoritesCount);
            
            var statusesCount =
                user.Element("statuses_count") == null ?
                    false :
                    ulong.TryParse(user.Element("statuses_count").Value, out tempStatusesCount);

            var notifications =
                user.Element("notifications") == null || 
                string.IsNullOrEmpty(user.Element("notifications").Value) ?
                DeviceType.None :
                    user.Element("notifications").Value == "false" ?
                    DeviceType.None :
                    (DeviceType)Enum.Parse(typeof(DeviceType), user.Element("notifications").Value, true);

            var isFollowing =
                user.Element("following") == null ||
                string.IsNullOrEmpty(user.Element("following").Value) ?
                    false :
                    bool.TryParse(user.Element("following").Value, out tempFollowingUsers);

            var status =
                user.Element("status");
            
            var statusDateParts =
                status == null ?
                    null :
                    status.Element("created_at").Value.Split(' ');
            
            var statusCreatedAtDate =
                statusDateParts == null ?
                    DateTime.MinValue :
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT",
                        statusDateParts[1],
                        statusDateParts[2],
                        statusDateParts[5],
                        statusDateParts[3]),
                        CultureInfo.InvariantCulture);
            
            var canParseTruncated =
                status == null ?
                    false :
                    bool.TryParse(status.Element("truncated").Value, out tempStatusTruncated);
            
            var canParseFavorited =
                status == null ?
                    false :
                    bool.TryParse(status.Element("favorited").Value, out tempStatusFavorited);

            var newUser = new User
            {
                ID = user.Element("id").Value,
                Name = user.Element("name").Value,
                ScreenName = user.Element("screen_name").Value,
                Location = user.Element("location").Value,
                Description = user.Element("description").Value,
                ProfileImageUrl = user.Element("profile_image_url").Value,
                URL = user.Element("url").Value,
                Protected = tempUserProtected,
                FollowersCount = tempFollowersCount,
                ProfileBackgroundColor = 
                    user.Element("profile_background_color") == null ?
                        string.Empty :
                        user.Element("profile_background_color").Value,
                ProfileTextColor = 
                    user.Element("profile_text_color") == null ?
                        string.Empty :
                        user.Element("profile_text_color").Value,
                ProfileLinkColor = 
                    user.Element("profile_link_color") == null ?
                        string.Empty :
                        user.Element("profile_link_color").Value,
                ProfileSidebarFillColor = 
                    user.Element("profile_sidebar_fill_color") == null ?
                        string.Empty :
                        user.Element("profile_sidebar_fill_color").Value,
                ProfileSidebarBorderColor = 
                    user.Element("profile_sidebar_border_color") == null ?
                        string.Empty :
                        user.Element("profile_sidebar_border_color").Value,
                FriendsCount = tempFriendsCount,
                CreatedAt = userCreatedAtDate,
                FavoritesCount = tempFavoritesCount,
                UtcOffset = 
                    user.Element("utc_offset") == null ?
                        string.Empty :
                        user.Element("utc_offset").Value,
                TimeZone = 
                    user.Element("time_zone") == null ?
                        string.Empty :
                        user.Element("time_zone").Value,
                ProfileBackgroundImageUrl = 
                    user.Element("profile_background_image_url") == null ?
                        string.Empty :
                        user.Element("profile_background_image_url").Value,
                ProfileBackgroundTile = 
                    user.Element("profile_background_tile") == null ?
                        string.Empty :
                        user.Element("profile_background_tile").Value,
                StatusesCount = tempStatusesCount,
                Notifications = notifications,
                Following = tempFollowingUsers,
                Status = 
                    status == null ?
                        null :
                        new Status
                        {
                            CreatedAt = statusCreatedAtDate,
                            ID = status.Element("id").Value,
                            Text = status.Element("text").Value,
                            Source = status.Element("source").Value,
                            Truncated = tempStatusTruncated,
                            InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                            InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                            Favorited = tempStatusFavorited,
                            InReplyToScreenName = status.Element("in_reply_to_screen_name").Value
                        }
            };

            return newUser;
        }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// user's Twitter ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// User ID for disambiguating when ID is screen name
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// user's screen name
        /// On Input - disambiguates when ID is User ID
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// name of user
        /// </summary>
        public string Name { get; set; }

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
        public ulong FollowersCount { get; set; }

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
        public ulong FriendsCount { get; set; }

        /// <summary>
        /// date and time when profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// number of favorites
        /// </summary>
        public ulong FavoritesCount { get; set; }

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
        public ulong StatusesCount { get; set; }

        /// <summary>
        /// type of device notifications
        /// </summary>
        public DeviceType Notifications { get; set; }

        /// <summary>
        /// is authenticated user following this user
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// current user status (valid only in user queries)
        /// </summary>
        public Status Status { get; set; }
    }
}