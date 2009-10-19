/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Web;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Reflection;

namespace LinqToTwitter
{
    /// <summary>
    /// manages access to Twitter API
    /// </summary>
    [Serializable]
    public class TwitterContext : IDisposable
    {
        #region TwitterContext initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        public TwitterContext()
            : this((ITwitterExecute)null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterAuthorization authorization)
            : this(new TwitterExecute(authorization), null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterAuthorization authorization, string baseUrl, string searchUrl)
            : this(new TwitterExecute(authorization), baseUrl, searchUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterExecute executor)
            : this(executor, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.  May be null.</param>
        /// <param name="baseUrl">Base url of Twitter API.  May be null to use the default "http://twitter.com/" value.</param>
        /// <param name="searchUrl">Base url of Twitter Search API.  May be null to use the default "http://search.twitter.com/" value.</param>
        public TwitterContext(ITwitterExecute execute, string baseUrl, string searchUrl)
        {
            TwitterExecutor = execute ?? new TwitterExecute();
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "https://twitter.com/" : baseUrl;
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "http://search.twitter.com/" : searchUrl;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the screen name of the user.
        /// </summary>
        public string UserName
        {
            get { return this.AuthorizedClient.ScreenName; }
        }

        private string baseUrl;

        /// <summary>
        /// base URL for accessing Twitter API
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return this.baseUrl;
            }

            set
            {
                this.baseUrl = value;
                try
                {
                    this.AuthorizedClient.AuthenticationTarget = value;
                }
                catch (NotSupportedException)
                {
                    // Some, like OAuth, don't use or support setting this property.  That's ok.
                }
            }
        }

        /// <summary>
        /// base URL for accessing Twitter Search API
        /// </summary>
        public string SearchUrl { get; set; }

        #endregion

        #region TwitterExecute Delegation

        //
        // The routines in this region delegate to TwitterExecute
        // which contains the methods for communicating with Twitter.
        // This is necessary so we can make the side-effect methods
        // more testable, using IoC.
        //

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                if (TwitterExecutor != null)
                {
                    return TwitterExecutor.UserAgent;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (TwitterExecutor != null)
                {
                    TwitterExecutor.UserAgent = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the read write timeout.
        /// </summary>
        /// <value>The read write timeout.</value>
        public int ReadWriteTimeout
        {
            get
            {
                if (TwitterExecutor != null)
                {
                    return TwitterExecutor.ReadWriteTimeout;
                }
                else
                {
                    return TwitterExecute.DefaultReadWriteTimeout;
                }
            }
            set
            {
                if (TwitterExecutor != null)
                {
                    TwitterExecutor.ReadWriteTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public int Timeout
        {
            get
            {
                if (TwitterExecutor != null)
                {
                    return TwitterExecutor.Timeout;
                }
                else
                {
                    return TwitterExecute.DefaultTimeout;
                }
            }
            set
            {
                if (TwitterExecutor != null)
                {
                    TwitterExecutor.Timeout = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the authorized client on the <see cref="ITwitterExecute"/> object.
        /// </summary>
        public ITwitterAuthorization AuthorizedClient {
            get { return this.TwitterExecutor.AuthorizedClient; }
            set { this.TwitterExecutor.AuthorizedClient = value; }
        }

        /// <summary>
        /// Methods for communicating with Twitter
        /// </summary>
        private ITwitterExecute TwitterExecutor { get; set; }

        #endregion

        #region TwitterQueryable objects

        /// <summary>
        /// enables access to Twitter account information, such as Verify Credentials and Rate Limit Status
        /// </summary>
        public TwitterQueryable<Account> Account
        {
            get
            {
                return new TwitterQueryable<Account>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter blocking information, such as Exists, Blocks, and IDs
        /// </summary>
        public TwitterQueryable<Blocks> Blocks
        {
            get
            {
                return new TwitterQueryable<Blocks>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter User messages, such as Friends and Followers
        /// </summary>
        public TwitterQueryable<DirectMessage> DirectMessage
        {
            get
            {
                return new TwitterQueryable<DirectMessage>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Favorites
        /// </summary>
        public TwitterQueryable<Favorites> Favorites
        {
            get
            {
                return new TwitterQueryable<Favorites>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Friendship info
        /// </summary>
        public TwitterQueryable<Friendship> Friendship
        {
            get
            {
                return new TwitterQueryable<Friendship>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Saved Searches
        /// </summary>
        public TwitterQueryable<SavedSearch> SavedSearch
        {
            get
            {
                return new TwitterQueryable<SavedSearch>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter SocialGraph to discover Friends and Followers
        /// </summary>
        public TwitterQueryable<SocialGraph> SocialGraph
        {
            get
            {
                return new TwitterQueryable<SocialGraph>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter SocialGraph to discover Friends and Followers
        /// </summary>
        public TwitterQueryable<Search> Search
        {
            get
            {
                return new TwitterQueryable<Search>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Status messages, such as Friends and Public
        /// </summary>
        public TwitterQueryable<Status> Status
        {
            get
            {
                return new TwitterQueryable<Status>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Trends, such as Trend, Current, Daily, and Weekly
        /// </summary>
        public TwitterQueryable<Trend> Trends
        {
            get
            {
                return new TwitterQueryable<Trend>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter User messages, such as Friends and Followers
        /// </summary>
        public TwitterQueryable<User> User
        {
            get
            {
                return new TwitterQueryable<User>(this);
            }
        }

        #endregion

        #region Response Headers

        //
        // response header constants
        //

        public const string XRateLimitLimitKey = "X-RateLimit-Limit";
        public const string XRateLimitRemainingKey = "X-RateLimit-Remaining";
        public const string XRateLimitResetKey = "X-RateLimit-Reset";
        public const string RetryAfterKey = "Retry-After";

        /// <summary>
        /// retrieves a specified response header, converting it to an int
        /// </summary>
        /// <param name="responseHeader">Response header to retrieve.</param>
        /// <returns>int value from response</returns>
        private int GetResponseHeaderAsInt(string responseHeader)
        {
            var headerVal = -1;

            if (ResponseHeaders != null &&
                ResponseHeaders.ContainsKey(responseHeader))
            {
                var headerValAsString = ResponseHeaders[responseHeader];

                int.TryParse(headerValAsString, out headerVal);
            }

            return headerVal;
        }

        /// <summary>
        /// Response headers from Twitter Queries
        /// </summary>
        public Dictionary<string, string> ResponseHeaders
        {
            get
            {
                if (TwitterExecutor != null)
                {
                    return TwitterExecutor.ResponseHeaders;
                }
                else
                {
                    return null;
                }
            }
       }

        /// <summary>
        /// Max number of requests per minute
        /// returned by the most recent query
        /// </summary>
        /// <remarks>
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int RateLimitCurrent
        {
            get
            {
                return GetResponseHeaderAsInt(XRateLimitLimitKey);
            }
        }

        /// <summary>
        /// Number of requests available until reset
        /// returned by the most recent query
        /// </summary>
        /// <remarks>
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int RateLimitRemaining
        {
            get
            {
                return GetResponseHeaderAsInt(XRateLimitRemainingKey);
            }
        }

        /// <summary>
        /// UTC time in ticks until rate limit resets
        /// returned by the most recent query
        /// </summary>
        /// <remarks>
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int RateLimitReset
        {
            get
            {
                return GetResponseHeaderAsInt(XRateLimitResetKey);
            }
        }

        /// <summary>
        /// UTC time in ticks until rate limit resets
        /// returned by the most recent search query 
        /// that fails with an HTTP 503
        /// </summary>
        /// <remarks>
        /// Returns -1 if information isn't available,
        /// i.e. you haven't exceeded search rate yet
        /// </remarks>
        public int RetryAfter
        {
            get
            {
                return GetResponseHeaderAsInt(RetryAfterKey);
            }
        }

        #endregion

        #region Twitter Query API

        /// <summary>
        /// Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <returns>list of objects with query results</returns>
        internal object Execute<T>(Expression expression, bool isEnumerable)
        {
            // request processor is specific to request type (i.e. Status, User, etc.)
            var reqProc = CreateRequestProcessor(expression);

            // get input parameters that go on the REST query URL
            var parameters = GetRequestParameters(expression, reqProc);

            // construct REST endpoint, based on input parameters
            var url = reqProc.BuildURL(parameters);

            // process request through Twitter
            var queryableList = TwitterExecutor.QueryTwitter(url, reqProc);

            // post-process results to perform non-twitter
            // specific processing using LINQ to Objects
            var finalResult = 
                ExecuteEnumerable<Status>(
                    queryableList as List<Status>, 
                    expression as MethodCallExpression,
                    isEnumerable);

            return finalResult;
        }

        /// <summary>
        /// Search the where clause for query parameters
        /// </summary>
        /// <param name="expression">Input query expression tree</param>
        /// <param name="reqProc">Processor specific to this request type</param>
        /// <returns>Name/value pairs of query parameters</returns>
        private static Dictionary<string, string> GetRequestParameters(Expression expression, IRequestProcessor reqProc)
        {
            Dictionary<string, string> parameters = null;

            // the where clause holds query arguments
            var whereExpression = new FirstWhereClauseFinder().GetFirstWhere(expression);

            if (whereExpression != null)
            {
                var lambdaExpression = (LambdaExpression)
                    ((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                // translate variable references in expression into constants
                lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                parameters = reqProc.GetParameters(lambdaExpression);
            }

            return parameters;
        }

        /// <summary>
        /// Main entry point for post-processing queries as LINQ to Objects
        /// </summary>
        /// <typeparam name="T">Type of object being processed</typeparam>
        /// <param name="list">List of objects to process</param>
        /// <param name="expr">Expression Tree with Lambda to process</param>
        /// <returns>List of processed items</returns>
        private object ExecuteEnumerable<T>(IEnumerable<T> list, MethodCallExpression expr, bool isEnumerable)
        {
            IEnumerable<T> nextResult = null;

            if (expr.Arguments[0].NodeType == ExpressionType.Call)
            {
                nextResult = (IEnumerable<T>)ExecuteEnumerable<T>(list, expr.Arguments[0] as MethodCallExpression, isEnumerable);
            }
            else
            {
                nextResult = list;
            }

            object operatorResult = null;

            var methodName = expr.Method.Name;
            var lambdaExpr = ((expr as MethodCallExpression).Arguments[1] as UnaryExpression).Operand as LambdaExpression;
            
            switch (methodName)
            {
                case "OrderBy":
                    operatorResult = ProcessOrderBy<T>(nextResult, lambdaExpr);
                    break;
                case "Select":
                    operatorResult = ProcessSelect<T>(nextResult, lambdaExpr);
                    break;
                case "Where":
                    operatorResult = list.Where((Func<T, bool>)lambdaExpr.Compile());
                    break;
                default:
                    break;
            }

            IList finalResult = null;

            if (operatorResult == null)
            {
                finalResult =
                    (IList)Activator.CreateInstance(
                        typeof(List<>)
                        .MakeGenericType(TypeSystem.GetElementType(list.GetType())));
                
                foreach (var item in (list as IEnumerable))
                {
                    finalResult.Add(item);
                }
            }
            else
            {
                finalResult =
                    (IList)Activator.CreateInstance(
                        typeof(List<>)
                        .MakeGenericType(TypeSystem.GetElementType(operatorResult.GetType())));
                
                foreach (var item in (operatorResult as IEnumerable))
                {
                    finalResult.Add(item);
                }
            }

            if (isEnumerable)
            {
                return finalResult;
            }
            else
            {
                return finalResult[0];
            }
        }

        /// <summary>
        /// Execute a projection on the list
        /// </summary>
        /// <typeparam name="T">Type being processed</typeparam>
        /// <param name="list">List to process</param>
        /// <param name="lambdaExpr">Lambda with projection</param>
        /// <returns>List of projected types</returns>
        private static object ProcessSelect<T>(IEnumerable<T> list, LambdaExpression lambdaExpr)
        {
            Type resultType = TypeSystem.GetElementType(lambdaExpr.Body.Type);

            var methodInfo =
                typeof(Enumerable)
                    .GetMethods()
                    .Where(
                        meth => meth.Name == "Select" &&
                        meth.GetGenericArguments().Count() == 2)
                    .FirstOrDefault();

            Type[] genericArguments = new Type[] { typeof(T), resultType };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);

            return genericMethodInfo.Invoke(null, new object[] { list, lambdaExpr.Compile() });
        }

        /// <summary>
        /// Execute an OrderBy operation on the list
        /// </summary>
        /// <typeparam name="T">Type of elements in list</typeparam>
        /// <param name="list">List of elements</param>
        /// <param name="lambdaExpr">Selector Lambda</param>
        /// <returns>List of ordered items</returns>
        private static IEnumerable<T> ProcessOrderBy<T>(IEnumerable<T> list, LambdaExpression lambdaExpr)
        {
            IEnumerable<T> operatorResult = null;

            switch (lambdaExpr.Body.Type.Name)
            {
                case "Boolean":
                    operatorResult = list.OrderBy((Func<T, bool>)lambdaExpr.Compile());
                    break;
                case "DateTime":
                    operatorResult = list.OrderBy((Func<T, DateTime>)lambdaExpr.Compile());
                    break;
                case "Decimal":
                    operatorResult = list.OrderBy((Func<T, decimal>)lambdaExpr.Compile());
                    break;
                case "Double":
                    operatorResult = list.OrderBy((Func<T, double>)lambdaExpr.Compile());
                    break;
                case "Int32":
                    operatorResult = list.OrderBy((Func<T, int>)lambdaExpr.Compile());
                    break;
                case "Single":
                    operatorResult = list.OrderBy((Func<T, float>)lambdaExpr.Compile());
                    break;
                case "String":
                    operatorResult = list.OrderBy((Func<T, string>)lambdaExpr.Compile());
                    break;
                case "UInt64":
                    operatorResult = list.OrderBy((Func<T, ulong>)lambdaExpr.Compile());
                    break;
                default:
                    break;
            }

            return operatorResult == null ? list : operatorResult.ToList();
        }

        /// <summary>
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        private IRequestProcessor CreateRequestProcessor(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("Expression passed to CreateRequestProcessor must not be null.");
            }

            string requestType = 
                TypeSystem
                    .GetElementType(
                        (expression as MethodCallExpression)
                        .Arguments[0].Type)
                        .Name;

            IRequestProcessor req;

            switch (requestType)
            {
                case "Account":
                    req = new AccountRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "Blocks":
                    req = new BlocksRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "DirectMessage":
                    req = new DirectMessageRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "Favorites":
                    req = new FavoritesRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "Friendship":
                    req = new FriendshipRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "SavedSearch":
                    req = new SavedSearchRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "SocialGraph":
                    req = new SocialGraphRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "Search":
                    req = new SearchRequestProcessor() { BaseUrl = SearchUrl };
                    break;
                case "Status":
                    req = new StatusRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                case "Trend":
                    req = new TrendRequestProcessor() { BaseUrl = SearchUrl };
                    break;
                case "User":
                    req = new UserRequestProcessor() { BaseUrl = BaseUrl };
                    break;
                default:
                    req = new StatusRequestProcessor() { BaseUrl = BaseUrl };
                    break;
            }

            Debug.Assert(req != null, "You you must assign a value to req.");

            return req;
        }

        #endregion

        #region Twitter Execution API

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public Status UpdateStatus(string status)
        {
            return UpdateStatus(status, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public Status UpdateStatus(string status, string inReplyToStatusID)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("status is a required parameter.");
            }

            if (status.Length > 140)
            {
                throw new ArgumentException("status length must be no more than 140 characters.", "status");
            }

            status = status.Substring(0, Math.Min(140, status.Length));

            var updateUrl = BaseUrl + "statuses/update.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID}
                    },
                    new StatusRequestProcessor());

            return (results as IList<Status>).FirstOrDefault();
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public Status DestroyStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IList<Status>).FirstOrDefault();
        }

        /// <summary>
        /// sends a new direct message to specified userr
        /// </summary>
        /// <param name="userID">id of user to send to</param>
        /// <param name="id">text to send</param>
        /// <returns>direct message element</returns>
        public DirectMessage NewDirectMessage(string userID, string text)
        {
            if (string.IsNullOrEmpty(userID))
            {
                throw new ArgumentException("userID is a required parameter.", "userID");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text is a required parameter.", "text");
            }

            if (text.Length > 140)
            {
                throw new ArgumentException("text must be no longer than 140 characters.", "text");
            }

            var newUrl = BaseUrl + "direct_messages/new.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", userID},
                        {"text", text}
                    },
                    new DirectMessageRequestProcessor());

            return (results as IList<DirectMessage>).FirstOrDefault();
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="id">id of direct message</param>
        /// <returns>direct message element</returns>
        public DirectMessage DestroyDirectMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "direct_messages/destroy/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new DirectMessageRequestProcessor());

            return (results as IList<DirectMessage>).FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public User CreateFriendship(string id, string userID, string screenName, bool follow)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl = null;

            if (!string.IsNullOrEmpty(id))
            {
                destroyUrl = BaseUrl + "friendships/create/" + id + ".xml";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                destroyUrl = BaseUrl + "friendships/create/" + userID + ".xml";
            }
            else
            {
                destroyUrl = BaseUrl + "friendships/create/" + screenName + ".xml";
            }

            var createParams = new Dictionary<string, string>
                {
                    { "user_id", userID },
                    { "screen_name", screenName }
                };
            
            // If follow exists in the parameter list, Twitter will
            // always treat it as true, even if the value is false;
            // Therefore, only add follow if it is true.
            if (follow)
            {
                createParams.Add("follow", "true");
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    createParams,
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public User DestroyFriendship(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl = null;

            if (!string.IsNullOrEmpty(id))
            {
                destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";
            }
            else
            {
                destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public Status CreateFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/create/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IList<Status>).FirstOrDefault();
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public Status DestroyFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/destroy/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IList<Status>).FirstOrDefault();
        }

        /// <summary>
        /// Disables notifications from specified user. (Notification Leave)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to disable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public User DisableNotifications(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string notificationsUrl = null;

            if (!string.IsNullOrEmpty(id))
            {
                notificationsUrl = BaseUrl + "notifications/leave/" + id + ".xml";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                notificationsUrl = BaseUrl + "notifications/leave/" + userID + ".xml";
            }
            else
            {
                notificationsUrl = BaseUrl + "notifications/leave/" + screenName + ".xml";
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Enables notifications from specified user (Notification Follow)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to enable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public User EnableNotifications(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string notificationsUrl = null;

            if (!string.IsNullOrEmpty(id))
            {
                notificationsUrl = BaseUrl + "notifications/follow/" + id + ".xml";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                notificationsUrl = BaseUrl + "notifications/follow/" + userID + ".xml";
            }
            else
            {
                notificationsUrl = BaseUrl + "notifications/follow/" + screenName + ".xml";
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="id">id of user to block</param>
        /// <returns>User that was unblocked</returns>
        public User CreateBlock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = BaseUrl + "blocks/create/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="id">id of user to unblock</param>
        /// <returns>User that was unblocked</returns>
        public User DestroyBlock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = BaseUrl + "blocks/destroy/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// sends a test message to twitter to check connectivity
        /// </summary>
        /// <returns>true</returns>
        public bool HelpTest()
        {
            var helpUrl = BaseUrl + "help/test.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    helpUrl,
                    new Dictionary<string, string>(),
                    new HelpRequestProcessor());

            return (results as IList<bool>).FirstOrDefault();
        }

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <returns>true</returns>
        public TwitterHashResponse EndAccountSession()
        {
            var accountUrl = BaseUrl + "account/end_session.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>(),
                    new AccountRequestProcessor());

            var acct = (results as IList<Account>).FirstOrDefault();

            if (acct != null)
            {
                return acct.EndSessionStatus;
            }
            else
            {
                throw new WebException("Unknown Twitter Response.");
            }
        }

        /// <summary>
        /// Updates notification device for account
        /// </summary>
        /// <param name="device">type of device to use</param>
        /// <returns>User info</returns>
        public User UpdateAccountDeliveryDevice(DeviceType device)
        {
            var accountUrl = BaseUrl + "account/update_delivery_device.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() }
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Update Twitter colors
        /// </summary>
        /// <remarks>
        /// The # character prefix is optional.  At least one color argument must be provided.
        /// </remarks>
        /// <param name="background">background color</param>
        /// <param name="text">text color</param>
        /// <param name="link">link color</param>
        /// <param name="sidebarFill">sidebar color</param>
        /// <param name="sidebarBorder">sidebar border color</param>
        /// <returns>User info with new colors</returns>
        public User UpdateAccountColors(string background, string text, string link, string sidebarFill, string sidebarBorder)
        {
            var accountUrl = BaseUrl + "account/update_profile_colors.xml";

            if (string.IsNullOrEmpty(background) &&
                string.IsNullOrEmpty(text) &&
                string.IsNullOrEmpty(link) &&
                string.IsNullOrEmpty(sidebarFill) &&
                string.IsNullOrEmpty(sidebarBorder))
            {
                throw new ArgumentException("At least one of the colors (background, text, link, sidebarFill, or sidebarBorder) must be provided as arguments, but none are specified.");
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "profile_background_color", background.TrimStart('#') },
                        { "profile_text_color", text.TrimStart('#') },
                        { "profile_link_color", link.TrimStart('#') },
                        { "profile_sidebar_fill_color", sidebarFill.TrimStart('#') },
                        { "profile_sidebar_border_color", sidebarBorder.TrimStart('#') }
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <returns>User with new image info</returns>
        public User UpdateAccountImage(string imageFilePath)
        {
            var accountUrl = BaseUrl + "account/update_profile_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var results = TwitterExecutor.PostTwitterFile(imageFilePath, null, accountUrl, new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <returns>User with new image info</returns>
        public User UpdateAccountBackgroundImage(string imageFilePath, bool tile)
        {
            var accountUrl = BaseUrl + "account/update_profile_background_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            Dictionary<string, string> parameters = null;

            // TODO: tile implementation doesn't seem to be working; numerous update background image problems reported in Twitter API; check again later - Joe
            if (tile)
            {
                parameters =
                        new Dictionary<string, string>
                {
                    { "tile", "true" }
                };
            }

            var results = TwitterExecutor.PostTwitterFile(imageFilePath, parameters, accountUrl, new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="email">Email Address</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <returns>User with new info</returns>
        public User UpdateAccountProfile(string name, string email, string url, string location, string description)
        {
            var accountUrl = BaseUrl + "account/update_profile.xml";

            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(email) &&
                string.IsNullOrEmpty(url) &&
                string.IsNullOrEmpty(location) &&
                string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("At least one of the colors (name, email, url, location, or description) must be provided as arguments, but none are specified.");
            }

            if (!string.IsNullOrEmpty(name) && name.Length > 20)
            {
                throw new ArgumentException("name must be no longer than 20 characters", "name");
            }

            if (!string.IsNullOrEmpty(email) && email.Length > 40)
            {
                throw new ArgumentException("email must be no longer than 40 characters", "email");
            }

            if (!string.IsNullOrEmpty(url) && url.Length > 100)
            {
                throw new ArgumentException("url must be no longer than 100 characters", "url");
            }

            if (!string.IsNullOrEmpty(location) && location.Length > 30)
            {
                throw new ArgumentException("location must be no longer than 30 characters", "location");
            }

            if (!string.IsNullOrEmpty(description) && description.Length > 160)
            {
                throw new ArgumentException("description must be no longer than 160 characters", "description");
            }

            var results =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "name", name },
                        { "email", email },
                        { "url", url },
                        { "location", location },
                        { "description", description }
                    },
                    new UserRequestProcessor());

            return (results as IList<User>).FirstOrDefault();
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public SavedSearch CreateSavedSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query is required.", "query");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/create.xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    },
                    new SavedSearchRequestProcessor());

            return (results as IList<SavedSearch>).FirstOrDefault();
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public SavedSearch DestroySavedSearch(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/destroy/" + id + ".xml";

            var results =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>(),
                    new SavedSearchRequestProcessor());

            return (results as IList<SavedSearch>).FirstOrDefault();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableExecutor = this.TwitterExecutor as IDisposable;
                if (disposableExecutor != null)
                {
                    disposableExecutor.Dispose();
                }
            }
        }

        #endregion
    }
}