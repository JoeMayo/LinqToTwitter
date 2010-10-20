/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        public static User CreateUser(XElement user)
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
            var tempFollowingUsers = false;
            var tempShowInlineMedia = false;
            var tempListedCount = 0;
            var tempFollowRequestSent = false;

            var canParseProtected =
                user.Element("protected") == null ?
                    false :
                    bool.TryParse(user.Element("protected").Value, out tempUserProtected);

            var followersCount =
                user.Element("followers_count") == null ?
                    false :
                    ulong.TryParse(user.Element("followers_count").Value, out tempFollowersCount);

            var friendsCount =
                user.Element("friends_count") == null ?
                    false :
                    ulong.TryParse(user.Element("friends_count").Value, out tempFriendsCount);

            var userDate = user.Element("created_at").Value;
            var userCreatedAtDate = String.IsNullOrEmpty(userDate) ?
                    DateTime.MinValue :
                    DateTime.ParseExact(
                        userDate,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

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

            var geoEnabled =
                user.Element("geo_enabled") == null ||
                string.IsNullOrEmpty(user.Element("geo_enabled").Value) ?
                    false :
                    bool.Parse(user.Element("geo_enabled").Value);

            var verified =
                user.Element("verified") == null ||
                string.IsNullOrEmpty(user.Element("verified").Value) ?
                    false :
                    bool.Parse(user.Element("verified").Value);

            var contributorsEnabled =
                user.Element("contributors_enabled") == null ||
                string.IsNullOrEmpty(user.Element("contributors_enabled").Value) ?
                    false :
                    bool.Parse(user.Element("contributors_enabled").Value);

            var isFollowing =
                user.Element("following") == null ||
                string.IsNullOrEmpty(user.Element("following").Value) ?
                    false :
                    bool.TryParse(user.Element("following").Value, out tempFollowingUsers);

            var showInlineMedia =
                user.Element("show_all_inline_media") == null ?
                    false :
                    bool.TryParse(user.Element("show_all_inline_media").Value, out tempShowInlineMedia);

            var listedCount =
                user.Element("listed_count") == null ?
                    false :
                    int.TryParse(user.Element("listed_count").Value, out tempListedCount);

            var followRequestSent =
                user.Element("follow_request_sent") == null ?
                    false :
                    bool.TryParse(user.Element("follow_request_sent").Value, out tempFollowRequestSent);

            var status =
                user.Element("status");

            var newUser = new User
            {
                Identifier = new UserIdentifier
                {
                    ID = user.Element("id") == null ? "0" : user.Element("id").Value,
                    UserID = user.Element("id") == null ? "0" : user.Element("id").Value,
                    ScreenName = user.Element("screen_name") == null ? "" : user.Element("screen_name").Value
                },
                Name = user.Element("name") == null ? "" : user.Element("name").Value,
                Location = user.Element("location") == null ? "" : user.Element("location").Value,
                Description = user.Element("description") == null ? "" : user.Element("description").Value,
                ProfileImageUrl = user.Element("profile_image_url") == null ? "" : user.Element("profile_image_url").Value,
                URL = user.Element("url") == null ? "" : user.Element("url").Value,
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
                GeoEnabled = geoEnabled,
                Verified = verified,
                ContributorsEnabled = contributorsEnabled,
                Following = tempFollowingUsers,
                ShowAllInlineMedia = tempShowInlineMedia,
                ListedCount = tempListedCount,
                FollowRequestSent = tempFollowRequestSent,
                Status = Status.CreateStatus(status),
                CursorMovement = Cursors.CreateCursors(GrandParentOrNull(user))
            };

            return newUser;
        }

        private static XElement GrandParentOrNull(XElement node)
        {
            if (node != null && node.Parent != null && node.Parent.Parent != null)
                return node.Parent.Parent;

            return null;
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
        /// <remarks>
        /// This was made obsolete for one API, but not Search. Therefore, we can't mark it as obsolete yet.
        /// </remarks>
        //[Obsolete("This property has been deprecated and will be ignored by Twitter. Please use Cursor/CursorMovement properties instead.")]
        public int Page { get; set; }

        /// <summary>
        /// Number of users to return for each page
        /// </summary>
        public int PerPage { get; set; }

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
        /// Used to identify suggested users category
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Query for User Search
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Contains Next and Previous cursors
        /// </summary>
        /// <remarks>
        /// This is read-only and returned with the response
        /// from Twitter. You use it by setting Cursor on the
        /// next request to indicate that you want to move to
        /// either the next or previous page.
        /// </remarks>
        [XmlIgnore]
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
        /// Supports Geo Tracking
        /// </summary>
        public bool GeoEnabled { get; set; }

        /// <summary>
        /// Is a verified account
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// Is contributors enabled on account?
        /// </summary>
        public bool ContributorsEnabled { get; set; }

        /// <summary>
        /// is authenticated user following this user
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// current user status (valid only in user queries)
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// User categories for Twitter Suggested Users
        /// </summary>
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Return results for specified language
        ///  Note: Twitter only supports a limited number of languages,
        ///  which include en, fr, de, es, it when this feature was added.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Indicates if user has inline media enabled
        /// </summary>
        public bool ShowAllInlineMedia { get; set; }

        /// <summary>
        /// Number of lists user is a member of
        /// </summary>
        public int ListedCount { get; set; }

        /// <summary>
        /// If authenticated user has requested to follow this use
        /// </summary>
        public bool FollowRequestSent { get; set; }
    }
}