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
                       "UserID",
                       "ScreenName",
                       "Page"
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

            UserType userType = RequestProcessorHelper.ParseQueryEnumType<UserType>(parameters["Type"]);

            switch (userType)
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
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            if (parameters.ContainsKey("UserID"))
            {
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                urlParams.Add("page=" + parameters["Page"]);
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
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var isRoot = twitterResponse.Name == "root";
            var responseItems = twitterResponse.Elements("root").ToList();

            string rootElement =
                isRoot || responseItems.Count > 0 ? "root" : "user";

            if (responseItems.Count == 0)
            {
                responseItems = twitterResponse.Elements(rootElement).ToList();
            }

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == rootElement)
            {
                responseItems.Add(twitterResponse);
            }

            var userList =
                from user in responseItems
                select new User().CreateUser(user);

            return userList.ToList();
        }

        #endregion
    }
}
