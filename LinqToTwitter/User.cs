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
    [Serializable]
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
                    false : 
                    bool.Parse(user.Element("notifications").Value);

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
                Identifier = new UserIdentifier
                {
                    ID = user.Element("id").Value,
                    UserID = user.Element("id").Value,
                    ScreenName = user.Element("screen_name").Value
                },
                Name = user.Element("name").Value,
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
                        },
                CursorMovement = new Cursors
                {
                    Next =
                        user.Element("next_cursor") == null ?
                            string.Empty :
                            user.Element("next_cursor").Value,
                    Previous =
                        user.Element("previous_cursor") == null ?
                            string.Empty :
                            user.Element("previous_cursor").Value
                }
            };

            return newUser;
        }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// Query user's Twitter ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Query User ID for disambiguating when ID is screen name
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Query screen name
        /// On Input - disambiguates when ID is User ID
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Identity properties of this specific user
        /// </summary>
        public UserIdentifier Identifier { get; set; }


        /// <summary>
        /// Page to return
        /// </summary>
        [Obsolete("This property has been deprecated and will be ignored by Twitter. Please use Cursor/CursorMovement properties instead.")]
        public int Page { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        public string Cursor { get; set; }

        /// <summary>
        /// Contains Next and Previous cursors
        /// </summary>
        /// <remarks>
        /// This is read-only and returned with the response
        /// from Twitter. You use it by setting Cursor on the
        /// next request to indicate that you want to move to
        /// either the next or previous page.
        /// </remarks>
        public Cursors CursorMovement { get; internal set; }

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
        public bool Notifications { get; set; }

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