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
using System.Xml.Linq;
using LinqToTwitter.Common;

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
            : this(new AnonymousAuthorization())
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
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.</param>
        /// <param name="baseUrl">Base url of Twitter API.  May be null to use the default "http://twitter.com/" value.</param>
        /// <param name="searchUrl">Base url of Twitter Search API.  May be null to use the default "http://search.twitter.com/" value.</param>
        public TwitterContext(ITwitterExecute execute, string baseUrl, string searchUrl)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            TwitterExecutor = execute;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "https://api.twitter.com/1/" : baseUrl;
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "http://search.twitter.com/" : searchUrl;
            StreamingUrl = "http://stream.twitter.com/1/";
            UserStreamUrl = "https://userstream.twitter.com/2/";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">OAuth provider</param>
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.</param>
        /// <param name="baseUrl">Base url of Twitter API.  May be null to use the default "http://twitter.com/" value.</param>
        /// <param name="searchUrl">Base url of Twitter Search API.  May be null to use the default "http://search.twitter.com/" value.</param>
        public TwitterContext(ITwitterAuthorization authorization, ITwitterExecute execute, string baseUrl, string searchUrl)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException("authorization");
            }

            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            TwitterExecutor = execute;
            TwitterExecutor.AuthorizedClient = authorization;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "https://api.twitter.com/1/" : baseUrl;
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

        /// <summary>
        /// base URL for accessing streaming APIs
        /// </summary>
        private string StreamingUrl { get; set; }

        /// <summary>
        /// base URL for accessing user stream APIs
        /// </summary>
        private string UserStreamUrl { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingUserName 
        {
            get { return TwitterExecutor.StreamingUserName; }
            set { TwitterExecutor.StreamingUserName = value; }
        }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        public string StreamingPassword
        {
            get { return TwitterExecutor.StreamingPassword; }
            set { TwitterExecutor.StreamingPassword = value; }
        }

        /// <summary>
        /// Assign the Log to the context
        /// </summary>
        public TextWriter Log
        {
            get { return TwitterExecute.Log; }
            set { TwitterExecute.Log = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Used to notify callers of changes in image upload progress
        /// </summary>
        public event EventHandler<TwitterProgressEventArgs> UploadProgressChanged
        {
            add
            {
                TwitterExecutor.UploadProgressChanged += value;
            }
            remove
            {
                TwitterExecutor.UploadProgressChanged -= value;
            }
        }

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
        /// Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        /// This is very useful for debugging
        /// </remarks>
        public string LastUrl
        {
            get { return this.TwitterExecutor.LastUrl; }
        }
        
        /// <summary>
        /// Methods for communicating with Twitter
        /// </summary>
        internal ITwitterExecute TwitterExecutor { get; set; }

        #endregion

        #region TwitterQueryable Entities

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
        /// enables access to Twitter Geo info
        /// </summary>
        public TwitterQueryable<Geo> Geo
        {
            get
            {
                return new TwitterQueryable<Geo>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter List info
        /// </summary>
        public TwitterQueryable<List> List
        {
            get
            {
                return new TwitterQueryable<List>(this);
            }
        }

        /// <summary>
        /// enables access to Raw Query Extensibility
        /// </summary>
        public TwitterQueryable<Raw> RawQuery
        {
            get
            {
                return new TwitterQueryable<Raw>(this);
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
        /// enables access to Twitter Status messages, such as Friends and Public
        /// </summary>
        public TwitterQueryable<Streaming> Streaming
        {
            get
            {
                return new TwitterQueryable<Streaming>(this);
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

        /// <summary>
        /// enables access to Twitter UserStream for streaming access to user info
        /// </summary>
        public TwitterQueryable<UserStream> UserStream
        {
            get
            {
                return new TwitterQueryable<UserStream>(this);
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
        public const string XFeatureRateLimitLimitKey = "X-FeatureRateLimit-Limit";
        public const string XFeatureRateLimitRemainingKey = "X-FeatureRateLimit-Remaining";
        public const string XFeatureRateLimitResetKey = "X-FeatureRateLimit-Reset";

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

        /// <summary>
        /// Max number of requests per minute
        /// returned by the most recent feature's query
        /// </summary>
        /// <remarks>
        /// Feature-specific rate limit that applies in conjunction with the
        /// main rate limit. Calls to certain APIs will count against its 
        /// feature-specific rate limit
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int FeatureRateLimitCurrent
        {
            get
            {
                return GetResponseHeaderAsInt(XFeatureRateLimitLimitKey);
            }
        }

        /// <summary>
        /// Number of requests available until reset
        /// returned by the most recent feature's query
        /// </summary>
        /// <remarks>
        /// Feature-specific rate limit that applies in conjunction with the
        /// main rate limit. Calls to certain APIs will count against its 
        /// feature-specific rate limit
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int FeatureRateLimitRemaining
        {
            get
            {
                return GetResponseHeaderAsInt(XFeatureRateLimitRemainingKey);
            }
        }

        /// <summary>
        /// UTC time in ticks until rate limit resets
        /// returned by the most recent feature's query
        /// </summary>
        /// <remarks>
        /// Feature-specific rate limit that applies in conjunction with the
        /// main rate limit. Calls to certain APIs will count against its 
        /// feature-specific rate limit
        /// Returns -1 if information isn't available,
        /// i.e. you haven't performed a query yet
        /// </remarks>
        public int FeatureRateLimitReset
        {
            get
            {
                return GetResponseHeaderAsInt(XFeatureRateLimitResetKey);
            }
        }
        #endregion

        #region Twitter Query API

        /// <summary>
        /// Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <returns>list of objects with query results</returns>
        public virtual object Execute<T>(Expression expression, bool isEnumerable)
        {
            // request processor is specific to request type (i.e. Status, User, etc.)
            var reqProc = CreateRequestProcessor<T>(expression);

            // get input parameters that go on the REST query URL
            var parameters = GetRequestParameters(expression, reqProc);

            // construct REST endpoint, based on input parameters
            var url = reqProc.BuildURL(parameters);

            string results = string.Empty;

            // process request through Twitter
            if (typeof(T) == typeof(Streaming) ||
                typeof(T) == typeof(UserStream))
            {
                results = TwitterExecutor.QueryTwitterStream(url);
            }
            else
            {
                results = TwitterExecutor.QueryTwitter(url);
            }

            // Transform results into objects
            var queryableList = reqProc.ProcessResults(results);

            // Copy the IEnumerable places to an IQueryable.
            var queryableItems = queryableList.AsQueryable<T>();

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            // -- Transforms IQueryable<T> into List<T>, which is (IEnumerable<T>)
            ExpressionTreeModifier<T> treeCopier = new ExpressionTreeModifier<T>(queryableItems);
            Expression newExpressionTree = treeCopier.CopyAndModify(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
                return queryableItems.Provider.CreateQuery(newExpressionTree);
            else
                return queryableItems.Provider.Execute(newExpressionTree);
        }

        /// <summary>
        /// Search the where clause for query parameters
        /// </summary>
        /// <param name="expression">Input query expression tree</param>
        /// <param name="reqProc">Processor specific to this request type</param>
        /// <returns>Name/value pairs of query parameters</returns>
        private static Dictionary<string, string> GetRequestParameters<T>(Expression expression, IRequestProcessor<T> reqProc)
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
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        private IRequestProcessor<T> CreateRequestProcessor<T>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("Expression passed to CreateRequestProcessor must not be null.");
            }

            string requestType = new MethodCallExpressionTypeFinder().GetGenericType(expression).Name;

            IRequestProcessor<T> req = null;

            switch (requestType)
            {
                case "Account":
                    req = new AccountRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Blocks":
                    req = new BlocksRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "DirectMessage":
                    req = new DirectMessageRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Favorites":
                    req = new FavoritesRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Friendship":
                    req = new FriendshipRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Geo":
                    req = new GeoRequestProcessor<T>() { BaseUrl = baseUrl };
                    break;
                case "List":
                    req = new ListRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Raw":
                    req = new RawRequestProcessor<T>() { BaseUrl = baseUrl };
                    break;
                case "SavedSearch":
                    req = new SavedSearchRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "SocialGraph":
                    req = new SocialGraphRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Search":
                    req = new SearchRequestProcessor<T>() { BaseUrl = SearchUrl };
                    break;
                case "Status":
                    req = new StatusRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "Streaming":
                    req = new StreamingRequestProcessor<T> 
                    { 
                        BaseUrl = StreamingUrl,
                        TwitterExecutor = TwitterExecutor
                    };
                    break;
                case "Trend":
                    req = new TrendRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "User":
                    req = new UserRequestProcessor<T>() { BaseUrl = BaseUrl };
                    break;
                case "UserStream":
                    req = new UserStreamRequestProcessor<T>
                    {
                        BaseUrl = UserStreamUrl,
                        TwitterExecutor = TwitterExecutor
                    };
                    break;
                default:
                    throw new ArgumentException("Type, " + requestType + " isn't a supported LINQ to Twitter entity.", "requestType");
            }

            Debug.Assert(req != null, "You you must assign a value to req.");

            return req;
        }

        #endregion

        #region Twitter Execution API

        #region Status Methods

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status)
        {
            return UpdateStatus(status, -1, -1, null, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string inReplyToStatusID)
        {
            return UpdateStatus(status, -1, -1, null, false, inReplyToStatusID);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude)
        {
            return UpdateStatus(status, latitude, longitude, null, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return UpdateStatus(status, latitude, longitude, null, displayCoordinates);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(status, latitude, longitude, null, displayCoordinates, inReplyToStatusID);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID)
        {
            return UpdateStatus(status, latitude, longitude, placeID, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID)
        {
            return UpdateStatus(status, latitude, longitude, placeID, false, inReplyToStatusID);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(status, latitude, longitude, placeID, displayCoordinates, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(status, -1, -1, placeID, displayCoordinates, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(status, -1, -1, placeID, displayCoordinates, inReplyToStatusID);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID},
                        {"lat", latitude == -1 ? null : latitude.ToString()},
                        {"long", longitude == -1 ? null : longitude.ToString()},
                        {"place_id", placeID},
                        {"display_coordinates", displayCoordinates.ToString()},
                    });

            List<Status> results = new StatusRequestProcessor<Status>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status DestroyStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>());

            List<Status> results = new StatusRequestProcessor<Status>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Direct Message Methods

        /// <summary>
        /// sends a new direct message to specified userr
        /// </summary>
        /// <param name="user">UserID or ScreenName of user to send to</param>
        /// <param name="id">Text to send</param>
        /// <returns>Direct message element</returns>
        public virtual DirectMessage NewDirectMessage(string user, string text)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentException("user is a required parameter.", "user");
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", user},
                        {"text", text}
                    });

            List<DirectMessage> results = new DirectMessageRequestProcessor<DirectMessage>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="id">id of direct message</param>
        /// <returns>direct message element</returns>
        public virtual DirectMessage DestroyDirectMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "direct_messages/destroy/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>());

            List<DirectMessage> results = new DirectMessageRequestProcessor<DirectMessage>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Friendship Methods

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public virtual User CreateFriendship(string id, string userID, string screenName, bool follow)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    createParams);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public virtual User DestroyFriendship(string id, string userID, string screenName)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="screenName">screen name of user to update</param>
        /// <returns>updated friend user info</returns>
        public virtual Friendship UpdateFriendshipSettings(string screenName, bool retweets, bool device)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentNullException("screenName", "screenName is a required parameter.");
            }

            string updateUrl = baseUrl + "friendships/update.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        { "screen_name", screenName },
                        { "retweets", retweets.ToString().ToLower() },
                        { "device", device.ToString().ToLower() }
                    });

            var results = new FriendshipRequestProcessor<Friendship>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Favorites Methods

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public virtual Status CreateFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/create/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>());

            List<Status> results = new StatusRequestProcessor<Status>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public virtual Status DestroyFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/destroy/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>());

            List<Status> results = new StatusRequestProcessor<Status>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Notifications Methods

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
        public virtual User DisableNotifications(string id, string userID, string screenName)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
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
        public virtual User EnableNotifications(string id, string userID, string screenName)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Block Methods

        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="id">id of user to block</param>
        /// <returns>User that was unblocked</returns>
        public virtual User CreateBlock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = BaseUrl + "blocks/create/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>());

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="id">id of user to unblock</param>
        /// <returns>User that was unblocked</returns>
        public virtual User DestroyBlock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = BaseUrl + "blocks/destroy/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>());

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Help Methods

        /// <summary>
        /// sends a test message to twitter to check connectivity
        /// </summary>
        /// <returns>true</returns>
        public virtual bool HelpTest()
        {
            var helpUrl = BaseUrl + "help/test.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    helpUrl,
                    new Dictionary<string, string>());

            List<bool> results = new HelpRequestProcessor<bool>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Account Methods

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <returns>true</returns>
        public virtual TwitterHashResponse EndAccountSession()
        {
            var accountUrl = BaseUrl + "account/end_session.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>());

            List<Account> results = new AccountRequestProcessor<Account>().ProcessResults(resultsXml);
            Account acct = results.FirstOrDefault();

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
        public virtual User UpdateAccountDeliveryDevice(DeviceType device)
        {
            var accountUrl = BaseUrl + "account/update_delivery_device.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() }
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
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
        public virtual User UpdateAccountColors(string background, string text, string link, string sidebarFill, string sidebarBorder)
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "profile_background_color", background.TrimStart('#') },
                        { "profile_text_color", text.TrimStart('#') },
                        { "profile_link_color", link.TrimStart('#') },
                        { "profile_sidebar_fill_color", sidebarFill.TrimStart('#') },
                        { "profile_sidebar_border_color", sidebarBorder.TrimStart('#') }
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
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
        public virtual User UpdateAccountImage(string imageFilePath)
        {
            var accountUrl = BaseUrl + "account/update_profile_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var resultsXml =
                TwitterExecutor.PostTwitterFile(imageFilePath, null, accountUrl);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="image">byte array of image to upload</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountImage(byte[] image, string fileName, string imageType)
        {
            var accountUrl = BaseUrl + "account/update_profile_image.xml";

            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("image is required.", "image");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is required.", "fileName");
            }

            if (string.IsNullOrEmpty(imageType))
            {
                throw new ArgumentException("imageType is required.", "imageType");
            }

            var resultsXml =
                TwitterExecutor.PostTwitterImage(image, null, accountUrl, fileName, imageType);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountBackgroundImage(string imageFilePath, bool tile, bool use)
        {
            var accountUrl = BaseUrl + "account/update_profile_background_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            Dictionary<string, string> parameters = null;

            if (tile)
            {
                parameters = new Dictionary<string, string>
                {
                    { "tile", tile.ToString().ToLower() },
                    { "use", use.ToString().ToLower() }
                };
            }

            var resultsXml =
                TwitterExecutor.PostTwitterFile(imageFilePath, parameters, accountUrl);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountBackgroundImage(byte[] image, string fileName, string imageType, bool tile, bool use)
        {
            var accountUrl = BaseUrl + "account/update_profile_background_image.xml";

            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("image is required.", "image");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is required.", "fileName");
            }

            if (string.IsNullOrEmpty(imageType))
            {
                throw new ArgumentException("imageType is required.", "imageType");
            }

            Dictionary<string, string> parameters = null;

            if (tile)
            {
                parameters = new Dictionary<string, string>
                {
                    { "tile", tile.ToString().ToLower() },
                    { "use", use.ToString().ToLower() }
                };
            }

            var resultsXml =
                TwitterExecutor.PostTwitterImage(image, parameters, accountUrl, fileName, imageType);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <returns>User with new info</returns>
        public virtual User UpdateAccountProfile(string name, string url, string location, string description)
        {
            var accountUrl = BaseUrl + "account/update_profile.xml";

            if (string.IsNullOrEmpty(name) &&
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

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "name", name },
                        { "url", url },
                        { "location", location },
                        { "description", description }
                    });

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Saved Search Methods

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch CreateSavedSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query is required.", "query");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/create.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    });

            List<SavedSearch> results = new SavedSearchRequestProcessor<SavedSearch>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch DestroySavedSearch(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/destroy/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>());

            List<SavedSearch> results = new SavedSearchRequestProcessor<SavedSearch>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Spam Methods

        /// <summary>
        /// lets logged-in user report spam
        /// </summary>
        /// <param name="id">id of alleged spammer</param>
        /// <param name="userID">user id of alleged spammer</param>
        /// <param name="screenName">screen name of alleged spammer</param>
        /// <returns>Alleged spammer user info</returns>
        public virtual User ReportSpam(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string reportSpamUrl = BaseUrl + "report_spam.xml";

            var createParams = new Dictionary<string, string>
                {
                    { "id", id },
                    { "user_id", userID },
                    { "screen_name", screenName }
                };

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    reportSpamUrl,
                    createParams);

            List<User> results = new UserRequestProcessor<User>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Retweet Methods

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status Retweet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var retweetUrl = BaseUrl + "statuses/retweet/" + id + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    retweetUrl,
                    new Dictionary<string, string>());

            List<Status> results = new StatusRequestProcessor<Status>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region List Methods

        /// <summary>
        /// Creates a new list
        /// </summary>
        /// <param name="screenName">name of user to create list for</param>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for new list</returns>
        public virtual List CreateList(string screenName, string listName, string mode, string description)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listName))
            {
                throw new ArgumentException("listName is required.", "listName");
            }

            var savedSearchUrl = BaseUrl + screenName + "/lists.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Modifies an existing list
        /// </summary>
        /// <param name="screenName">name of user to modify list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for modified list</returns>
        public virtual List UpdateList(string screenName, string listID, string listName, string mode, string description)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/lists/" + listID + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="screenName">name of user to delete list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <returns>List info for deleted list</returns>
        public virtual List DeleteList(string screenName, string listID)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/lists/" + listID + ".xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "_method", "DELETE" }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a user as a list member
        /// </summary>
        /// <param name="screenName">name of user to add member to list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="memberID">ID of member to add</param>
        /// <returns>List info for list member added to</returns>
        public virtual List AddMemberToList(string screenName, string listID, string memberID)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/members.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "id", memberID }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a list of users to a list
        /// </summary>
        /// <remarks>
        /// Original code contributed by zorgster/CodePlex.com
        /// </remarks>
        /// <param name="screenName">name of user to add member to list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="members">List of IDs of members to add</param>
        /// <returns>List info for list member added to - up to 100 Members at a time can be added</returns>
        public virtual List AddMemberRangeToList(string screenName, string listID, List<string> members)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            if (members.Count > 100)
            {
                throw new ArgumentException("Max users is 100 at a time.", "members");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/members/create_all.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "screen_name", string.Join(",", members.ToArray()) }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a list of users to a list
        /// </summary>
        /// <remarks>
        /// Original code contributed by zorgster/CodePlex.com
        /// </remarks>
        /// <param name="screenName">name of user to add member to list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="memberIDs">List of IDs of members to add</param>
        /// <returns>List info for list member added to - up to 100 Members at a time can be added</returns>
        public virtual List AddMemberRangeToList(string screenName, string listID, List<ulong> memberIDs)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            if (memberIDs.Count > 100)
            {
                throw new ArgumentException("Max users is 100 at a time.", "members");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/members/create_all.xml";

            string[] idStrings =
                (from id in memberIDs
                 select id.ToString())
                .ToArray();

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", string.Join(",", idStrings) }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Removes a user as a list member
        /// </summary>
        /// <param name="screenName">name of user to remove member from list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="memberID">ID of member to remove</param>
        /// <returns>List info for list member removed from</returns>
        public virtual List DeleteMemberFromList(string screenName, string listID, string memberID)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/members.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "id", memberID },
                        { "_method", "DELETE" }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="screenName">name of user to add subscription to list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <returns>List info for list subscribed to</returns>
        public virtual List SubscribeToList(string screenName, string listID)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/subscribers.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>());

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="screenName">name of user to remove subscription from list for</param>
        /// <param name="listID">ID or slug of list</param>
        /// <returns>List info for list subscription removed from</returns>
        public virtual List UnsubscribeFromList(string screenName, string listID)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("screenName is required.", "screenName");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("listID is required.", "listID");
            }

            var savedSearchUrl = BaseUrl + screenName + "/" + listID + @"/subscribers.xml";

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "_method", "DELETE" }
                    });

            List<List> results = new ListRequestProcessor<List>().ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        #endregion

        #region Raw Requests

        public string ExecuteRaw(string queryString, Dictionary<string, string> parameters)
        {
            string rawUrl = BaseUrl.TrimEnd('/') + "/" + queryString.TrimStart('/');

            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    rawUrl,
                    parameters);

            return resultsXml;
        }

        #endregion

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