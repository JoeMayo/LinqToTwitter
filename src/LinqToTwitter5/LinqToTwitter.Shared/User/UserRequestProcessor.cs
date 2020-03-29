using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter User requests.
    /// </summary>
    public class UserRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        const string ScreenNameOrUserID = "ScreenNameOrUserID";
        const string ScreenNameListOrUserIdList = "ScreenNameListOrUserIdList";

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of user request (i.e. Friends, Followers, or Show)
        /// </summary>
        internal UserType Type { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        internal ulong UserID { get; set; }

        /// <summary>
        /// Comma-separated list of user IDs (e.g. for Lookup query)
        /// </summary>
        internal string UserIdList { get; set; }

        /// <summary>
        /// user's screen name
        /// On Input - disambiguates when ID is User ID
        /// </summary>
        internal string ScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of screen names (e.g. for Lookup query)
        /// </summary>
        public string ScreenNameList { get; set; }

        /// <summary>
        /// page number of results to retrieve
        /// </summary>
        internal int Page { get; set; }

        /// <summary>
        /// Number of users to return for each page
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        internal long Cursor { get; set; }

        /// <summary>
        /// Used to identify suggested users category
        /// </summary>
        internal string Slug { get; set; }

        /// <summary>
        /// Query for User Search
        /// </summary>
        internal string Query { get; set; }

        /// <summary>
        /// Supports various languages
        /// </summary>
        internal string Lang { get; set; }

        /// <summary>
        /// Add entities to results
        /// </summary>
        internal bool IncludeEntities { get; set; }

        /// <summary>
        /// Remove status from results
        /// </summary>
        internal bool SkipStatus { get; set; }

        /// <summary>
        /// Size for UserProfileImage query
        /// </summary>
        internal ProfileImageSize ImageSize { get; set; }

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
                       "UserID",
                       "UserIdList",
                       "ScreenName",
                       "ScreenNameList",
                       "Page",
                       "Count",
                       "Cursor",
                       "Slug",
                       "Query",
                       "Lang",
                       "IncludeEntities",
                       "SkipStatus",
                       "ImageSize"
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<UserType>(parameters["Type"]);

            switch (Type)
            {
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
                case UserType.Contributees:
                    return BuildContributeesUrl(parameters);
                case UserType.Contributors:
                    return BuildContributorsUrl(parameters);
                case UserType.BannerSizes:
                    return BuildBannerSizesUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildContributorsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users/contributors.json");
            var urlParams = req.RequestParameters;

            if (!(parameters.ContainsKey("ScreenName") || parameters.ContainsKey("UserID")) ||
                (parameters.ContainsKey("ScreenName") && parameters.ContainsKey("UserID")))
                throw new ArgumentException("Query must contain one of either ScreenName or UserID parameters, but not both.", ScreenNameOrUserID);

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
            }

            return req;
        }
 
        Request BuildContributeesUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("ScreenName") || parameters.ContainsKey("UserID")) ||
                (parameters.ContainsKey("ScreenName") && parameters.ContainsKey("UserID")))
                throw new ArgumentException("Query must contain one of either ScreenName or UserID parameters, but not both.", ScreenNameOrUserID);

            var req = new Request(BaseUrl + "users/contributees.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds a URL to perform a user search
        /// </summary>
        /// <param name="parameters">Query, Page, and Count</param>
        /// <returns>URL for performing user search</returns>
        Request BuildSearchUrl(Dictionary<string, string> parameters)
        {
            const string QueryParam = "Query";
            if (!parameters.ContainsKey("Query"))
                throw new ArgumentException("Query parameter is required.", QueryParam);

            var req = new Request(BaseUrl + "users/search.json");
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

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds a url for performing lookups
        /// </summary>
        /// <param name="parameters">Either UserID or ScreenName</param>
        /// <returns>URL for performing lookups</returns>
        Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            if (!(parameters.ContainsKey("ScreenNameList") || parameters.ContainsKey("UserIdList")) ||
                (parameters.ContainsKey("ScreenNameList") && parameters.ContainsKey("UserIdList")))
                throw new ArgumentException("Query must contain one of either ScreenNameList or UserIdList parameters, but not both.", ScreenNameListOrUserIdList);

            var req = new Request(BaseUrl + "users/lookup.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ScreenNameList"))
            {
                ScreenNameList = parameters["ScreenNameList"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenNameList"]));
            }

            if (parameters.ContainsKey("UserIdList"))
            {
                UserIdList = parameters["UserIdList"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserIdList"].Replace(" ", "")));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Builds url for getting users that belong to a suggestion category
        /// </summary>
        /// <param name="parameters">Contains Slug. Required.</param>
        /// <returns>Url for query + slug</returns>
        Request BuildUsersInCategoryUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Slug"))
                throw new ArgumentException("Slug parameter is required.", "Slug");

            Slug = parameters["Slug"];

            var req = new Request(BaseUrl + "users/suggestions/" + Slug + ".json");
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
        Request BuildCategoriesUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users/suggestions.json");
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
        Request BuildCategoryStatusUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Slug"))
                throw new ArgumentNullException("Slug", "You must set the Slug property, which is the suggested category.");

            Slug = parameters["Slug"];
            var req = new Request(BaseUrl + "users/suggestions/" + Slug.ToLower() + "/members.json");

            return req;
        }

        /// <summary>
        /// builds a url to show user info
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("UserID") &&
                !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("Parameters must include either UserID or ScreenName.", ScreenNameOrUserID);
            }

            if (parameters.ContainsKey("UserID") && string.IsNullOrWhiteSpace(parameters["UserID"]))
            {
                throw new ArgumentNullException("UserID", "If specified, UserID can't be null or an empty string.");
            }

            if (parameters.ContainsKey("ScreenName") && string.IsNullOrWhiteSpace(parameters["ScreenName"]))
            {
                throw new ArgumentNullException("ScreenName", "If specified, ScreenName can't be null or an empty string.");
            }

            var req = new Request(BaseUrl + "users/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            return req;
        }

        Request BuildBannerSizesUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users/profile_banner.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("UserID") &&
                !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("Parameters must include either UserID or ScreenName.", ScreenNameOrUserID);
            }

            if (parameters.ContainsKey("UserID") && string.IsNullOrWhiteSpace(parameters["UserID"]))
            {
                throw new ArgumentNullException("UserID", "If specified, UserID can't be null or an empty string.");
            }

            if (parameters.ContainsKey("ScreenName") && string.IsNullOrWhiteSpace(parameters["ScreenName"]))
            {
                throw new ArgumentNullException("ScreenName", "If specified, ScreenName can't be null or an empty string.");
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            return req;
        }

        /// <summary>
        /// Transforms Twitter response into List of User
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            List<User> userList = null;

            JsonData userJson = JsonMapper.ToObject(responseJson);

            switch (Type)
            {
                case UserType.Show:
                    userList = HandleSingleUserResponse(userJson);
                    break;
                case UserType.Categories:
                    userList = HandleMultipleCategoriesResponse(userJson);
                    break;
                case UserType.Category:
                    userList = HandleSingleCategoryResponse(userJson);
                    break;
                case UserType.Contributees:
                case UserType.Contributors:
                case UserType.CategoryStatus:
                case UserType.Lookup:
                case UserType.Search:
                    userList = HandleMultipleUserResponse(userJson);
                    break;
                case UserType.BannerSizes:
                    userList = HandleBannerSizesResponse(userJson);
                    break;
                default:
                    userList = new List<User>();
                    break;
            }

            foreach(var user in userList)
            {
                user.Type = Type;
                user.UserID = UserID;
                user.UserIdList = UserIdList;
                user.ScreenName = ScreenName;
                user.ScreenNameList = ScreenNameList;
                user.Page = Page;
                user.Count = Count;
                user.Cursor = Cursor;
                user.Slug = Slug;
                user.Lang = Lang;
                user.Query = Query;
                user.IncludeEntities = IncludeEntities;
                user.SkipStatus = SkipStatus;
                user.ImageSize = ImageSize;
            }

            return userList.OfType<T>().ToList();
        }
  
        List<User> HandleSingleUserResponse(JsonData userJson)
        {
            List<User> userList = new List<User> { new User(userJson) };
            return userList;
        }
  
        List<User> HandleMultipleCategoriesResponse(JsonData userJson)
        {
            List<User> userList = new List<User>
            {
                new User
                {
                    Categories =
                        (from JsonData cat in userJson
                         select new Category(cat))
                        .ToList()
                }
            };

            return userList;
        }
  
        List<User> HandleSingleCategoryResponse(JsonData userJson)
        {
            List<User> userList = new List<User>
            {
                new User
                {
                    Categories = new List<Category> { new Category(userJson) }
                }
            };

            return userList;
        }
  
        List<User> HandleMultipleUserResponse(JsonData userJson)
        {
            List<User> userList =
                (from JsonData user in userJson
                 select new User(user))
                .ToList();

            return userList;
        }

        List<User> HandleBannerSizesResponse(JsonData userJson)
        {
            var sizes = userJson.GetValue<JsonData>("sizes");
            var userList = new List<User>
            {
                new User
                {
                    BannerSizes =     
                        (from key in (sizes as IDictionary<string, JsonData>).Keys as List<string>
                         let sizesKey = sizes.GetValue<JsonData>(key)
                         select new BannerSize
                         {
                             Label = key,
                             Width = sizesKey.GetValue<int>("w"),
                             Height = sizesKey.GetValue<int>("h"),
                             Url = sizesKey.GetValue<string>("url")
                         })
                        .ToList()
                }
            };

            return userList;
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData userJson = JsonMapper.ToObject(responseJson);

            List<User> user = HandleSingleUserResponse(userJson);

            return user.Single().ItemCast(default(T));
        }
    }
}
