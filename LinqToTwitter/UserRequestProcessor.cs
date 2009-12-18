using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

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
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        private UserType Type { get; set; }

        /// <summary>
        /// user's Twitter ID
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        /// User ID for disambiguating when ID is screen name
        /// </summary>
        private string UserID { get; set; }

        /// <summary>
        /// user's screen name
        /// On Input - disambiguates when ID is User ID
        /// </summary>
        private string ScreenName { get; set; }

        /// <summary>
        /// page number of results to retrieve
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        private string Cursor { get; set; }
        
        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<User>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "UserID",
                       "ScreenName",
                       "Page",
                       "Cursor"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                url = BuildFriendsUrl(null);
                return url;
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<UserType>(parameters["Type"]);

            switch (Type)
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
            var url = BaseUrl + "users/show.json";

            if (parameters != null)
            {
                url = BuildFriendsAndFollowersUrlParameters(parameters, url);
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

            if (!parameters.ContainsKey("ID") && 
                !parameters.ContainsKey("UserID") && 
                !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("Parameters must include at least one of ID, UserID, or ScreenName.");
            }

            var urlParams = new List<string>();

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add("cursor=" + parameters["Cursor"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
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

        // TODO: received when twitter was down - write a test
        //<hash>
        //  <request></request>
        //  <error>Twitter is down for maintenance. It will return in about an hour.</error>
        //</hash>

        /// <summary>
        /// transforms XML into IList of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IList of User</returns>
        public virtual IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var isRoot = twitterResponse.Name == "root";
            var responseItems = twitterResponse.Elements("root").ToList();

            string rootElement =
                isRoot || responseItems.Count > 0 ? "root" : "user";

            if (responseItems.Count == 0)
            {
                responseItems = twitterResponse.Elements(rootElement).ToList();
            }

            if (twitterResponse.Element("users") != null)
            {
                responseItems =
                    (from user in twitterResponse.Element("users").Elements("user").ToList()
                     select user)
                     .ToList();
            }

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == rootElement)
            {
                responseItems.Add(twitterResponse);
            }

            var users =
                from user in responseItems
                select new User().CreateUser(user);

            var userList = users.ToList();

            userList.ForEach(
                user =>
                {
                    user.Type = Type;
                    user.ID = ID;
                    user.UserID = UserID;
                    user.ScreenName = ScreenName;
                    user.Page = Page;
                    user.Cursor = Cursor;
                });

            return userList.ToList();
        }

        #endregion
    }
}
