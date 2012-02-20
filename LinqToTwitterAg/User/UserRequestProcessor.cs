using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter User requests
    /// </summary>
    public class UserRequestProcessor<T> : IRequestProcessor<T>
    {
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
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<UserType>(parameters["Type"]);

            switch (Type)
            {
                case UserType.Followers:
                    return BuildFollowersUrl(parameters);
                case UserType.Friends:
                    return BuildFriendsUrl(parameters);
                case UserType.Show:
                    return BuildShowUrl(parameters);
                case UserType.Categories:
                    return BuildCategoriesUrl(parameters);
                case UserType.Category:
                    return BuildUsersInCategoryUrl(parameters);
                case UserType.CategoryStatus:
                    return BuildCategoryStatusUrl(parameters);
                case UserType.Lookup:
                    return BuildLookupUrl(parameters);
                case UserType.Search:
                    return BuildSearchUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds a URL to perform a user search
        /// </summary>
        /// <param name="parameters">Query, Page, and PerPage</param>
        /// <returns>URL for performing user search</returns>
        private Request BuildSearchUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Query"))
                throw new ArgumentException("Query parameter is required.");

            var req = new Request(BaseUrl + "users/search.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Query"))
            {
                Query = parameters["Query"];
                urlParams.Add(new QueryParameter("q", parameters["Query"]));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            if (parameters.ContainsKey("PerPage"))
            {
                PerPage = int.Parse(parameters["PerPage"]);
                urlParams.Add(new QueryParameter("per_page", parameters["PerPage"]));
            }

            return req;
        }

        /// <summary>
        /// Builds a url for performing lookups
        /// </summary>
        /// <param name="parameters">Either UserID or ScreenName</param>
        /// <returns>URL for performing lookups</returns>
        private Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("ScreenName") || parameters.ContainsKey("UserID")) ||
                (parameters.ContainsKey("ScreenName") && parameters.ContainsKey("UserID")))
                throw new ArgumentException("Query must contain one of either ScreenName or UserID parameters, but not both.");

            var req = new Request(BaseUrl + "users/lookup.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            return req;
        }

        /// <summary>
        /// Builds url for getting users that belong to a suggestion category
        /// </summary>
        /// <param name="parameters">Contains Slug. Required.</param>
        /// <returns>Url for query + slug</returns>
        private Request BuildUsersInCategoryUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Slug"))
                throw new ArgumentException("Slug parameter is required.", "Slug");

            Slug = parameters["Slug"];

            var req = new Request(BaseUrl + "users/suggestions/" + Slug + ".xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Lang"))
            {
                Lang = parameters["Lang"];
                urlParams.Add(new QueryParameter("lang", parameters["Lang"]));
            }

            return req;
        }

        /// <summary>
        /// Builds a url to get suggested user categories
        /// </summary>
        /// <param name="parameters">Not used</param>
        /// <returns>Url for suggested user categories</returns>
        private Request BuildCategoriesUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users/suggestions.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Lang"))
            {
                Lang = parameters["Lang"];
                urlParams.Add(new QueryParameter("lang", parameters["Lang"]));
            }

            return req;
        }

        /// <summary>
        /// Builds a url to get tweets of users in a suggested category
        /// </summary>
        /// <param name="parameters">Reads Slug param</param>
        /// <returns>Url for category statuses</returns>
        private Request BuildCategoryStatusUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Slug"))
                throw new ArgumentNullException("Slug", "You must set the Slug property, which is the suggested category.");

            Slug = parameters["Slug"];
            var req = new Request(BaseUrl + "users/suggestions/" + Slug.ToLower() + "/members.xml");

            return req;
        }

        /// <summary>
        /// builds a url to show user info
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            // TODO: The format of this used to be show.json and now Twitter offers an xml option
            // check to see if there's dead code in ProcessRequest based on the old json to xml translation - Joe
            return BuildFriendsAndFollowersUrlParameters(parameters, "users/show.xml");
        }

        /// <summary>
        /// builds an url for getting a list of user's friends
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildFriendsUrl(Dictionary<string, string> parameters)
        {
            return BuildFriendsAndFollowersUrlParameters(parameters, "statuses/friends.xml");
        }

        /// <summary>
        /// builds an url for getting a list of user's followers
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildFollowersUrl(Dictionary<string, string> parameters)
        {
            return BuildFriendsAndFollowersUrlParameters(parameters, "statuses/followers.xml");
        }

        /// <summary>
        /// common code for building parameter list for friends and followers urls
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        internal Request BuildFriendsAndFollowersUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (parameters == null)
            {
                return new Request(BaseUrl + url);
            }

            if (!parameters.ContainsKey("ID") &&
                !parameters.ContainsKey("UserID") &&
                !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("Parameters must include at least one of ID, UserID, or ScreenName.");
            }

            if (parameters.ContainsKey("UserID") && string.IsNullOrEmpty(parameters["UserID"]))
            {
                throw new ArgumentNullException("UserID", "If specified, UserID can't be null or an empty string.");
            }

            if (parameters.ContainsKey("ScreenName") && string.IsNullOrEmpty(parameters["ScreenName"]))
            {
                throw new ArgumentNullException("ScreenName", "If specified, ScreenName can't be null or an empty string.");
            }

            if (parameters.ContainsKey("ID") && string.IsNullOrEmpty(parameters["ID"]))
            {
                throw new ArgumentNullException("ID", "If specified, ID can't be null or an empty string.");
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// transforms XML into IList of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<users></users>";
            }

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
                    user.Query = Query;
                });

            return userList.OfType<T>().ToList();
        }
    }
}
