using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Web;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter User requests
    /// </summary>
    public class UserRequestProcessor<T> : IRequestProcessor<T>
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
        /// Number of users to return for each page
        /// </summary>
        private int PerPage { get; set; }

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
        /// Used to identify suggested users category
        /// </summary>
        private string Slug { get; set; }

        /// <summary>
        /// Query for User Search
        /// </summary>
        private string Query { get; set; }

        /// <summary>
        /// Supports various languages
        /// </summary>
        private string Lang { get; set; }
      
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
                       "PerPage",
                       "Cursor",
                       "Slug",
                       "Query",
                       "Lang"
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
                throw new ArgumentException("You must set Type.", "Type");
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
                case UserType.Categories:
                    url = BuildCategoriesUrl(parameters);
                    break;
                case UserType.Category:
                    url = BuildUsersInCategoryUrl(parameters);
                    break;
                case UserType.CategoryStatus:
                    url = BuildCategoryStatusUrl(parameters);
                    break;
                case UserType.Lookup:
                    url = BuildLookupUrl(parameters);
                    break;
                case UserType.Search:
                    url = BuildSearchUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// Builds a URL to perform a user search
        /// </summary>
        /// <param name="parameters">Query, Page, and PerPage</param>
        /// <returns>URL for performing user search</returns>
        private string BuildSearchUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Query"))
            {
                throw new ArgumentException("Query parameter is required.");
            }

            string url = BaseUrl + "users/search.xml";
            var urlParams = new List<string>();

            if (parameters.ContainsKey("Query"))
            {
                Query = parameters["Query"];
                urlParams.Add("q=" + HttpUtility.UrlEncode(parameters["Query"]));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("PerPage"))
            {
                PerPage = int.Parse(parameters["PerPage"]);
                urlParams.Add("per_page=" + parameters["PerPage"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// Builds a url for performing lookups
        /// </summary>
        /// <param name="parameters">Either UserID or ScreenName</param>
        /// <returns>URL for performing lookups</returns>
        private string BuildLookupUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("ScreenName") || parameters.ContainsKey("UserID")) ||
                (parameters.ContainsKey("ScreenName") && parameters.ContainsKey("UserID")))
            {
                throw new ArgumentException("Query must contain one of either ScreenName or UserID parameters, but not both.");
            }

            string url = BaseUrl + "users/lookup.xml?";

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                url += "screen_name=" + parameters["ScreenName"];
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                url += "user_id=" + parameters["UserID"];
            }

            return url;
        }

        /// <summary>
        /// Builds url for getting users that belong to a suggestion category
        /// </summary>
        /// <param name="parameters">Contains Slug. Required.</param>
        /// <returns>Url for query + slug</returns>
        private string BuildUsersInCategoryUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Slug"))
            {
                throw new ArgumentException("Slug parameter is required.", "Slug");
            }

            Slug = parameters["Slug"];

            string url = BaseUrl + "users/suggestions/" + parameters["Slug"] + ".xml";

            if (parameters.ContainsKey("Lang"))
            {
                url += "?lang=" + parameters["Lang"];
                Lang = parameters["Lang"];
            }

            return url;
        }

        /// <summary>
        /// Builds a url to get suggested user categories
        /// </summary>
        /// <param name="parameters">Not used</param>
        /// <returns>Url for suggested user categories</returns>
        private string BuildCategoriesUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl + "users/suggestions.xml";

            if (parameters.ContainsKey("Lang"))
            {
                url += "?lang=" + parameters["Lang"];
                Lang = parameters["Lang"];
            }

            return url;
        }

        /// <summary>
        /// Builds a url to get tweets of users in a suggested category
        /// </summary>
        /// <param name="parameters">Reads Slug param</param>
        /// <returns>Url for category statuses</returns>
        private string BuildCategoryStatusUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("Slug"))
            {
                Slug = parameters["Slug"]; 
            }
            else
            {
                throw new ArgumentNullException("Slug", "You must set the Slug property, which is the suggested category.");
            }

            return BaseUrl + "users/suggestions/" + Slug.ToLower() + "/members.xml";
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

        /// <summary>
        /// transforms XML into IList of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            var userList = new List<User>();
            var categories = new List<Category>();

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

            if (twitterResponse.Name == "suggestions" && twitterResponse.Elements("user").Count() == 0)
            {
                userList.Add(new User());

                categories =
                    (from cat in twitterResponse.Elements("category")
                     select Category.CreateCategory(cat))
                     .ToList();
            }

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == rootElement)
            {
                responseItems.Add(twitterResponse);
            }

            var users =
                (from user in responseItems
                 select User.CreateUser(user))
                .ToList();

            userList.AddRange(users);

            userList.ForEach(
                user =>
                {
                    user.Type = Type;
                    user.ID = ID;
                    user.UserID = UserID;
                    user.ScreenName = ScreenName;
                    user.Page = Page;
                    user.Cursor = Cursor;
                    user.Slug = Slug;
                    user.Categories = categories;
                    user.Lang = Lang;
                });

            return userList.OfType<T>().ToList();
        }

        #endregion
    }
}
