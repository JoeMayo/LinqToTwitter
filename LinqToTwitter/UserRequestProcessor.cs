using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter User requests
    /// </summary>
    public class UserRequestProcessor : IRequestProcessor
    {
        #region IRequestProcessor Members

        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<User>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "Page",
                       "Email"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                url = BuildFriendsUrl(null);
                return url;
            }

            switch ((UserType)Enum.ToObject(typeof(UserType), int.Parse(parameters["Type"])))
            {
                case UserType.Followers:
                    url = BuildFollowersUrl(parameters);
                    break;
                case UserType.Friends:
                    url = BuildFriendsUrl(parameters);
                    break;
                case UserType.Show:
                    url = BuildShowUrl(parameters);
                    break;
                default:
                    url = BuildFriendsUrl(null);
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds a url to show user info
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        private string BuildShowUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "users/show.xml";

            if (parameters.ContainsKey("Email"))
            {
                url += "?email=" + parameters["Email"];
            }
            else if (parameters.ContainsKey("ID"))
            {
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }
            else
            {
                // Twitter requires one or the other of ID or Email
                throw new InvalidQueryException("You must provide either a user ID or an Email for a User Show request.");
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting a list of user's friends
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private string BuildFriendsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/friends.xml";

            if (parameters != null)
            {
                url = BuildFriendsAndFollowersUrlParameters(parameters, url); 
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting a list of user's followers
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private string BuildFollowersUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/followers.xml";

            if (parameters != null)
            {
                url = BuildFriendsAndFollowersUrlParameters(parameters, url); 
            }

            return url;
        }

        /// <summary>
        /// common code for building parameter list for friends and followers urls
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        private string BuildFriendsAndFollowersUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (parameters == null)
            {
                return url;
            }

            if (parameters.ContainsKey("ID"))
            {
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            if (parameters.ContainsKey("Page"))
            {
                url += "?page=" + parameters["Page"];
            }

            return url;
        }

        //<user>
        //  <id>15411837</id>
        //  <name>Joe Mayo</name>
        //  <screen_name>JoeMayo</screen_name>
        //  <location>Denver, CO</location>
        //  <description>Author/entrepreneur, specializing in custom .NET software development</description>
        //  <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
        //  <url>http://www.csharp-station.com</url>
        //  <protected>false</protected>
        //  <followers_count>25</followers_count>
        //  <profile_background_color>C6E2EE</profile_background_color>
        //  <profile_text_color>663B12</profile_text_color>
        //  <profile_link_color>1F98C7</profile_link_color>
        //  <profile_sidebar_fill_color>DAECF4</profile_sidebar_fill_color>
        //  <profile_sidebar_border_color>C6E2EE</profile_sidebar_border_color>
        //  <friends_count>1</friends_count>
        //  <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
        //  <favourites_count>0</favourites_count>
        //  <utc_offset>-25200</utc_offset>
        //  <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        //  <profile_background_image_url>http://static.twitter.com/images/themes/theme2/bg.gif</profile_background_image_url>
        //  <profile_background_tile>false</profile_background_tile>
        //  <statuses_count>81</statuses_count>
        //  <status>
        //    <created_at>Sun Jan 18 21:58:24 +0000 2009</created_at>
        //    <id>1128977017</id>
        //    <text>New schedule for #SoCalCodeCamp by @DanielEgan - http://tinyurl.com/9gv5zp</text>
        //    <source>web</source>
        //    <truncated>false</truncated>
        //    <in_reply_to_status_id></in_reply_to_status_id>
        //    <in_reply_to_user_id></in_reply_to_user_id>
        //    <favorited>false</favorited>
        //    <in_reply_to_screen_name></in_reply_to_screen_name>
        //  </status>
        //</user>

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public IQueryable ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("user").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "user")
            {
                responseItems.Add(twitterResponse);
            }

            var tempUserProtected = false;
            var tempFollowersCount = 0;
            var tempFriendsCount = 0;
            var tempFavoritesCount = 0;
            var tempStatusesCount = 0;
            var tempStatusTruncated = false;
            var tempStatusFavorited = false;

            var userList =
                from user in responseItems
                let canParseProtected = 
                    bool.TryParse(user.Element("protected").Value, out tempUserProtected)
                let followersCount = 
                    int.TryParse(user.Element("followers_count").Value, out tempFollowersCount)
                let friendsCount =
                    user.Element("friends_count") == null ? 
                        false :
                        int.TryParse(user.Element("friends_count").Value, out tempFriendsCount)
                let userDateParts =
                    user.Element("created_at") == null ?
                        null :
                        user.Element("created_at").Value.Split(' ')
                let userCreatedAtDate =
                    userDateParts == null ?
                        DateTime.MinValue :
                        DateTime.Parse(
                            string.Format("{0} {1} {2} {3} GMT",
                            userDateParts[1],
                            userDateParts[2],
                            userDateParts[5],
                            userDateParts[3]))
                let favoritesCount =
                    user.Element("favourites_count") == null ? 
                        false :
                        int.TryParse(user.Element("favourites_count").Value, out tempFavoritesCount)
                let statusesCount =
                    user.Element("statuses_count") == null ?
                        false :
                        int.TryParse(user.Element("statuses_count").Value, out tempStatusesCount)
                let status = 
                    user.Element("status")
                let statusDateParts =
                    status == null ?
                        null :
                        status.Element("created_at").Value.Split(' ')
                let statusCreatedAtDate =
                    statusDateParts == null ?
                        DateTime.MinValue :
                        DateTime.Parse(
                            string.Format("{0} {1} {2} {3} GMT",
                            statusDateParts[1],
                            statusDateParts[2],
                            statusDateParts[5],
                            statusDateParts[3]))
                let canParseTruncated =
                    status == null ?
                        false :
                        bool.TryParse(status.Element("truncated").Value, out tempStatusTruncated)
                let canParseFavorited =
                    status == null ?
                        false :
                        bool.TryParse(status.Element("favorited").Value, out tempStatusFavorited)
                select new User
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

            var queryableUser = userList.AsQueryable<User>();
            return queryableUser;
        }

        #endregion
    }
}
