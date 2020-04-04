/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 * *********************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// information for a twitter user
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class User
    {
        public User() {}
        public User(JsonData user)
        {
            if (user == null) return;

            BannerSizes = new List<BannerSize>();
            Categories = new List<Category>();
            UserIDResponse = user.GetValue<ulong>("id").ToString(CultureInfo.InvariantCulture);
            ScreenNameResponse = user.GetValue<string>("screen_name");
            Name = user.GetValue<string>("name");
            Location = user.GetValue<string>("location");
            Description = user.GetValue<string>("description");
            ProfileImageUrl = user.GetValue<string>("profile_image_url");
            ProfileImageUrlHttps = user.GetValue<string>("profile_image_url_https");
            Url = user.GetValue<string>("url");
            Entities = new UserEntities(user.GetValue<JsonData>("entities"));
            Protected = user.GetValue<bool>("protected");
            ProfileUseBackgroundImage = user.GetValue<bool>("profile_use_background_image");
            IsTranslator = user.GetValue<bool>("is_translator");
            FollowersCount = user.GetValue<int>("followers_count");
            DefaultProfile = user.GetValue<bool>("default_profile");
            ProfileBackgroundColor = user.GetValue<string>("profile_background_color");
            LangResponse = user.GetValue<string>("lang");
            ProfileTextColor = user.GetValue<string>("profile_text_color");
            ProfileLinkColor = user.GetValue<string>("profile_link_color");
            ProfileSidebarFillColor = user.GetValue<string>("profile_sidebar_fill_color");
            ProfileSidebarBorderColor = user.GetValue<string>("profile_sidebar_border_color");
            FriendsCount = user.GetValue<int>("friends_count");
            DefaultProfileImage = user.GetValue<bool>("default_profile_image");
            CreatedAt = user.GetValue<string>("created_at").GetDate(DateTime.MinValue);
            FavoritesCount = user.GetValue<int>("favourites_count");
            UtcOffset = user.GetValue<int>("utc_offset");
            TimeZone = user.GetValue<string>("time_zone");
            ProfileBackgroundImageUrl = user.GetValue<string>("profile_background_image_url");
            ProfileBackgroundImageUrlHttps = user.GetValue<string>("profile_background_image_url_https");
            ProfileBackgroundTile = user.GetValue<bool>("profile_background_tile");
            ProfileBannerUrl = user.GetValue<string>("profile_banner_url");
            StatusesCount = user.GetValue<int>("statuses_count");
            Notifications = user.GetValue<bool>("notifications");
            GeoEnabled = user.GetValue<bool>("geo_enabled");
            Verified = user.GetValue<bool>("verified");
            ContributorsEnabled = user.GetValue<bool>("contributors_enabled");
            Following = user.GetValue<bool>("following");
            ShowAllInlineMedia = user.GetValue<bool>("show_all_inline_media");
            ListedCount = user.GetValue<int>("listed_count");
            FollowRequestSent = user.GetValue<bool>("follow_request_sent");
            Status = new Status(user.GetValue<JsonData>("status"));
            CursorMovement = new Cursors(user);
            Email = user.GetValue<string>("email");
        }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// Query User ID
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// Comma-separated list of user IDs (e.g. for Lookup query)
        /// </summary>
        public string UserIdList { get; set; }

        /// <summary>
        /// Query screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of screen names (e.g. for Lookup queries)
        /// </summary>
        public string ScreenNameList { get; set; }

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
        public int Count { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        public long Cursor { get; set; }

        /// <summary>
        /// Used to identify suggested users category
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Query for User Search
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Add entities to results (default: true)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Remove status from results
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Query User ID
        /// </summary>
        public string UserIDResponse { get; set; }

        /// <summary>
        /// Query screen name
        /// </summary>
        public string ScreenNameResponse { get; set; }

        /// <summary>
        /// Size for UserProfileImage query
        /// </summary>
        public ProfileImageSize ImageSize { get; set; }

        /// <summary>
        /// Set to TweetMode.Extended to receive 280 characters in Status.FullText property
        /// </summary>
        public TweetMode TweetMode { get; set; }

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
        /// user's image for use on HTTPS secured pages
        /// </summary>
        public string ProfileImageUrlHttps { get; set; }

        /// <summary>
        /// user's image is a defaulted placeholder
        /// </summary>
        public bool DefaultProfileImage{ get; set; }

        /// <summary>
        /// user's URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Entities connected to the <see cref="User"/>
        /// </summary>
        public UserEntities Entities { get; set; }

        /// <summary>
        /// user's profile has not been configured (is just defaults)
        /// </summary>
        public bool DefaultProfile { get; set; }

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
        public int UtcOffset { get; set; }

        /// <summary>
        /// Time Zone
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// URL of profile background image
        /// </summary>
        public string ProfileBackgroundImageUrl { get; set; }

        /// <summary>
        /// URL of profile background image for use on HTTPS secured pages
        /// </summary>
        public string ProfileBackgroundImageUrlHttps { get; set; }

        /// <summary>
        /// Title of profile background
        /// </summary>
        public bool ProfileBackgroundTile { get; set; }

        /// <summary>
        /// Should we use the profile background image?
        /// </summary>
        public bool ProfileUseBackgroundImage { get; set; }

        /// <summary>
        /// number of status updates user has made
        /// </summary>
        public int StatusesCount { get; set; }

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
        /// Is this a translator?
        /// </summary>
        public bool IsTranslator { get; set; }

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
        /// Input param for Category queries
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Return results for specified language
        ///  Note: Twitter only supports a limited number of languages,
        ///  which include en, fr, de, es, it when this feature was added.
        /// </summary>
        public string LangResponse { get; set; }

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

        /// <summary>
        /// Response from ProfileImage query
        /// </summary>
        public string ProfileImage { get; set; }

        /// <summary>
        /// Url of Profile Banner image.
        /// </summary>
        public string ProfileBannerUrl { get; set; }

        /// <summary>
        /// Available sizes to use in account banners.
        /// </summary>
        public List<BannerSize> BannerSizes { get; set; }

        /// <summary>
        /// User's email-address (null if not filled in on app is
        /// lacking whitelisting)
        /// </summary>
        public string Email { get; set; }
    }
}