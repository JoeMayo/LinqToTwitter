/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 ***********************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

using LinqToTwitter.Common;
#if SILVERLIGHT
using System.Net.Browser;
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// manages access to Twitter API
    /// </summary>
    public class TwitterContext : IDisposable
    {
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
        public const string DateKey = "Date";

        //
        // Parameter constants
        //

        const string ListOrSlugParam = "ListIdOrSlug";
        const string OwnerIDOrOwnerScreenNameParam = "OwnerIdOrOwnerScreenName";
        const string UserIDOrScreenNameParam = "UserIdOrScreenName";

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        public TwitterContext()
            : this(new AnonymousAuthorizer())
        {
            BaseUrl = "https://api.twitter.com/1/";
            SearchUrl = "https://search.twitter.com/";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterAuthorizer authorization)
            : this(new TwitterExecute(authorization), null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="baseUrl">Overwrites default base URL</param>
        /// <param name="searchUrl">Overwrites default search URL</param>
        public TwitterContext(ITwitterAuthorizer authorization, string baseUrl, string searchUrl)
            : this(new TwitterExecute(authorization), baseUrl, searchUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="executor">Can be mocked for testing</param>
        public TwitterContext(ITwitterExecute executor)
            : this(executor, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.</param>
        /// <param name="baseUrl">Base url of Twitter API.  May be null to use the default "https://api.twitter.com/1/" value.</param>
        /// <param name="searchUrl">Base url of Twitter Search API.  May be null to use the default "https://search.twitter.com/" value.</param>
        public TwitterContext(ITwitterExecute execute, string baseUrl, string searchUrl)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            TwitterExecutor = execute;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "https://api.twitter.com/1/" : baseUrl;
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "https://search.twitter.com/" : searchUrl;
            StreamingUrl = "https://stream.twitter.com/1/";
            UserStreamUrl = "https://userstream.twitter.com/2/";
            SiteStreamUrl = "https://sitestream.twitter.com/2b/";
            UploadUrl = "https://upload.twitter.com/1/";

#if SILVERLIGHT && !WINDOWS_PHONE

            if (System.Windows.Application.Current.IsRunningOutOfBrowser)
            {
                WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
                WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
            }
            else
            {
                WebRequest.RegisterPrefix("http://", WebRequestCreator.BrowserHttp);
                WebRequest.RegisterPrefix("https://", WebRequestCreator.BrowserHttp);
            }

#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">OAuth provider</param>
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.</param>
        /// <param name="baseUrl">Base url of Twitter API.  May be null to use the default "https://api.twitter.com/1/" value.</param>
        /// <param name="searchUrl">Base url of Twitter Search API.  May be null to use the default "https://search.twitter.com/" value.</param>
        public TwitterContext(ITwitterAuthorizer authorization, ITwitterExecute execute, string baseUrl, string searchUrl)
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
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "https://search.twitter.com/" : searchUrl;

#if SILVERLIGHT
            WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
#endif
        }

        /// <summary>
        /// Gets the screen name of the user.
        /// </summary>
        public string UserName
        {
            get { return AuthorizedClient.ScreenName; }
        }

        /// <summary>
        /// base URL for accessing Twitter API
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// base URL for accessing Twitter Search API
        /// </summary>
        public string SearchUrl { get; set; }

        /// <summary>
        /// base URL for accessing streaming APIs
        /// </summary>
        public string StreamingUrl { get; set; }

        /// <summary>
        /// base URL for accessing user stream APIs
        /// </summary>
        public string UserStreamUrl { get; set; }

        /// <summary>
        /// base URL for accessing site stream APIs
        /// </summary>
        public string SiteStreamUrl { get; set; }

        /// <summary>
        /// twitter endpoint for update with media requests
        /// </summary>
        public string UploadUrl { get; set; }

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

        public string RawResult { get; set; }

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
                return string.Empty;
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
                return TwitterExecute.DefaultReadWriteTimeout;
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
                return TwitterExecute.DefaultTimeout;
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
        public ITwitterAuthorizer AuthorizedClient
        {
            get { return TwitterExecutor.AuthorizedClient; }
            set { TwitterExecutor.AuthorizedClient = value; }
        }

        /// <summary>
        /// Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        /// This is very useful for debugging
        /// </remarks>
        public string LastUrl
        {
            get { return TwitterExecutor.LastUrl; }
        }
        
        /// <summary>
        /// Methods for communicating with Twitter
        /// </summary>
        internal ITwitterExecute TwitterExecutor { get; set; }

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
        /// enables access to Twitter Help info
        /// </summary>
        public TwitterQueryable<Help> Help
        {
            get
            {
                return new TwitterQueryable<Help>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Legal info
        /// </summary>
        public TwitterQueryable<Legal> Legal
        {
            get
            {
                return new TwitterQueryable<Legal>(this);
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
        /// enables access to Related Results Query Extensibility
        /// </summary>
        public TwitterQueryable<RelatedResults> RelatedResults
        {
            get
            {
                return new TwitterQueryable<RelatedResults>(this);
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

        /// <summary>
        /// retrieves a specified response header, converting it to an int
        /// </summary>
        /// <param name="responseHeader">Response header to retrieve.</param>
        /// <returns>int value from response</returns>
        private int GetResponseHeaderAsInt(string responseHeader)
        {
            var headerVal = -1;
            var headers = ResponseHeaders;

            if (headers != null &&
                headers.ContainsKey(responseHeader))
            {
                var headerValAsString = headers[responseHeader];

                int.TryParse(headerValAsString, out headerVal);
            }

            return headerVal;
        }

        /// <summary>
        /// retrieves a specified response header, converting it to a DateTime
        /// </summary>
        /// <param name="responseHeader">Response header to retrieve.</param>
        /// <returns>DateTime value from response</returns>
        /// <remarks>Expects a string like: Sat, 26 Feb 2011 01:12:08 GMT</remarks>
        private DateTime? GetResponseHeaderAsDateTime(string responseHeader)
        {
            DateTime? headerVal = null;
            var headers = ResponseHeaders;

            if (headers != null &&
                headers.ContainsKey(responseHeader))
            {
                var headerValAsString = headers[responseHeader];
                DateTime value;

                if (DateTime.TryParse(headerValAsString,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                        out value))
                    headerVal = value;
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
                return null;
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

        /// <summary>
        /// Gets the response header Date and converts to a nullable-DateTime
        /// </summary>
        /// <remarks>
        /// Returns null if the headers don't contain a valid Date value
        /// i.e. you haven't performed a query yet or not convertable
        /// </remarks>
        public DateTime? TwitterDate
        {
            get
            {
                return GetResponseHeaderAsDateTime(DateKey);
            }
        }

        /// <summary>
        /// Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <param name="isEnumerable">Indicates whether expression is enumerable</param>
        /// <returns>list of objects with query results</returns>
        public virtual object Execute<T>(Expression expression, bool isEnumerable)
            where T: class
        {
            // request processor is specific to request type (i.e. Status, User, etc.)
            var reqProc = CreateRequestProcessor<T>(expression);

            // get input parameters that go on the REST query URL
            var parameters = GetRequestParameters(expression, reqProc);

            // construct REST endpoint, based on input parameters
            var request = reqProc.BuildURL(parameters);

            string results;

            // process request through Twitter
            if (typeof(T) == typeof(Streaming) ||
                typeof(T) == typeof(UserStream))
            {
                results = TwitterExecutor.QueryTwitterStream(request);
            }
            else
            {
                results = TwitterExecutor.QueryTwitter(request, reqProc);
            }

            RawResult = results;

            // Transform results into objects
            var queryableList = reqProc.ProcessResults(results);

            // Copy the IEnumerable entities to an IQueryable.
            var queryableItems = queryableList.AsQueryable();

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            // -- Transforms IQueryable<T> into List<T>, which is (IEnumerable<T>)
            var treeCopier = new ExpressionTreeModifier<T>(queryableItems);
            Expression newExpressionTree = treeCopier.CopyAndModify(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
                return queryableItems.Provider.CreateQuery(newExpressionTree);

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

        protected internal virtual IRequestProcessor<T> CreateRequestProcessor<T>()
            where T : class
        {
            string requestType = typeof(T).Name;

            IRequestProcessor<T> req = CreateRequestProcessor<T>(requestType);

            return req;
        }

        /// <summary>
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        internal IRequestProcessor<T> CreateRequestProcessor<T>(Expression expression)
            where T: class
        {
            if (expression == null)
            {
                const string nullExpressionMessage = "Expression passed to CreateRequestProcessor must not be null.";
                throw new ArgumentNullException(nullExpressionMessage);
            }

            string requestType = new MethodCallExpressionTypeFinder().GetGenericType(expression).Name;

            IRequestProcessor<T> req = CreateRequestProcessor<T>(requestType);
            return req;
        }

        protected internal IRequestProcessor<T> CreateRequestProcessor<T>(string requestType)
            where T : class
        {
            var baseUrl = BaseUrl;
            IRequestProcessor<T> req;

            switch (requestType)
            {
                case "Account":
                    req = new AccountRequestProcessor<T>();
                    break;
                case "Blocks":
                    req = new BlocksRequestProcessor<T>();
                    break;
                case "DirectMessage":
                    req = new DirectMessageRequestProcessor<T>();
                    break;
                case "Favorites":
                    req = new FavoritesRequestProcessor<T>();
                    break;
                case "Friendship":
                    req = new FriendshipRequestProcessor<T>();
                    break;
                case "Geo":
                    req = new GeoRequestProcessor<T>();
                    break;
                case "Help":
                    req = new HelpRequestProcessor<T>();
                    break;
                case "Legal":
                    req = new LegalRequestProcessor<T>();
                    break;
                case "List":
                    req = new ListRequestProcessor<T>();
                    break;
                case "Raw":
                    req = new RawRequestProcessor<T>();
                    break;
                case "RelatedResults":
                    req = new RelatedResultsRequestProcessor<T>();
                    break;
                case "SavedSearch":
                    req = new SavedSearchRequestProcessor<T>();
                    break;
                case "SocialGraph":
                    req = new SocialGraphRequestProcessor<T>();
                    break;
                case "Search":
                    baseUrl = SearchUrl;
                    req = new SearchRequestProcessor<T>();
                    break;
                case "Status":
                    req = new StatusRequestProcessor<T>();
                    break;
                case "Streaming":
                    baseUrl = StreamingUrl;
                    req = new StreamingRequestProcessor<T>
                    {
                        TwitterExecutor = TwitterExecutor
                    };
                    break;
                case "Trend":
                    req = new TrendRequestProcessor<T>();
                    break;
                case "User":
                    req = new UserRequestProcessor<T>();
                    break;
                case "UserStream":
                    baseUrl = null; // don't set that..
                    req = new UserStreamRequestProcessor<T>
                    {
                        UserStreamUrl = UserStreamUrl,
                        SiteStreamUrl = SiteStreamUrl,
                        TwitterExecutor = TwitterExecutor
                    };
                    break;
                default:
                    throw new ArgumentException("Type, " + requestType + " isn't a supported LINQ to Twitter entity.", "requestType");
            }

            if (baseUrl != null)
                req.BaseUrl = baseUrl;

            return req;
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status)
        {
            return UpdateStatus(status, false, -1, -1, null, false, null, null);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, null, false, null, null);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, -1, -1, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string inReplyToStatusID)
        {
            return UpdateStatus(status, false, -1, -1, null, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string inReplyToStatusID)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, null, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, -1, -1, null, false, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, null, false, inReplyToStatusID, callback);
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
            return UpdateStatus(status, false, latitude, longitude, null, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, false, null, callback);
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
            return UpdateStatus(status, false, latitude, longitude, null, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, null, displayCoordinates, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, displayCoordinates, callback);
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
            return UpdateStatus(status, false, latitude, longitude, null, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, null, displayCoordinates, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, null, displayCoordinates, inReplyToStatusID, callback);
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
            return UpdateStatus(status, false, latitude, longitude, placeID, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, placeID, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, false, null, callback);
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
            return UpdateStatus(status, false, latitude, longitude, placeID, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, placeID, false, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, false, inReplyToStatusID, callback);
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
            return UpdateStatus(status, false, latitude, longitude, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, latitude, longitude, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, displayCoordinates, null, callback);
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
            return UpdateStatus(status, false, -1, -1, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, -1, -1, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, placeID, displayCoordinates, null, callback);
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
            return UpdateStatus(status, false, -1, -1, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, false, -1, -1, placeID, displayCoordinates, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(status, wrapLinks, -1, -1, placeID, displayCoordinates, inReplyToStatusID, callback);
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
            return UpdateStatus(status, false, latitude, longitude, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(status, wrapLinks, latitude, longitude, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public virtual Status UpdateStatus(string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("status is a required parameter.");
            }

            var updateUrl = BaseUrl + "statuses/update.xml";

            var reqProc = new StatusRequestProcessor<Status>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID},
                        {"lat", latitude == -1 ? null : latitude.ToString(CultureInfo.InvariantCulture)},
                        {"long", longitude == -1 ? null : longitude.ToString(CultureInfo.InvariantCulture)},
                        {"place_id", placeID},
                        {"display_coordinates", displayCoordinates.ToString(CultureInfo.InvariantCulture)},
                        {"wrap_links", wrapLinks ? true.ToString(CultureInfo.InvariantCulture) : null }
                    },
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status DestroyStatus(string id)
        {
            return DestroyStatus(id, null);
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status DestroyStatus(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to follow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public virtual User CreateFriendship(string id, string userID, string screenName, bool follow)
        {
            return CreateFriendship(id, userID, screenName, follow, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to follow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public virtual User CreateFriendship(string id, string userID, string screenName, bool follow, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl;

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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    createParams,
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to unfollow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public virtual User DestroyFriendship(string id, string userID, string screenName)
        {
            return DestroyFriendship(id, userID, screenName, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to unfollow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public virtual User DestroyFriendship(string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl;

            if (!string.IsNullOrEmpty(id))
            {
                destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";
            }
            else
            {
                destroyUrl = BaseUrl + "friendships/destroy.xml";
            }

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public virtual Friendship UpdateFriendshipSettings(string screenName, bool retweets, bool device)
        {
            return UpdateFriendshipSettings(screenName, retweets, device, null);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>updated friend user info</returns>
        public virtual Friendship UpdateFriendshipSettings(string screenName, bool retweets, bool device, Action<TwitterAsyncResponse<Friendship>> callback)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentNullException("screenName", "screenName is a required parameter.");
            }

            string updateUrl = BaseUrl + "friendships/update.xml";

            var reqProc = new FriendshipRequestProcessor<Friendship>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        { "screen_name", screenName },
                        { "retweets", retweets.ToString(CultureInfo.InvariantCulture).ToLower() },
                        { "device", device.ToString(CultureInfo.InvariantCulture).ToLower() }
                    },
                    reqProc);

            var results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public virtual Status CreateFavorite(string id)
        {
            return CreateFavorite(id, null);
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public virtual Status CreateFavorite(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/create/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public virtual Status DestroyFavorite(string id)
        {
            return DestroyFavorite(id, null);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public virtual Status DestroyFavorite(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/destroy/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
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
        public virtual User DisableNotifications(string id, string userID, string screenName)
        {
            return DisableNotifications(id, userID, screenName, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Specified user info</returns>
        public virtual User DisableNotifications(string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string notificationsUrl;

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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return EnableNotifications(id, userID, screenName, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Specified user info</returns>
        public virtual User EnableNotifications(string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string notificationsUrl;

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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <returns>true</returns>
        public virtual TwitterHashResponse EndAccountSession()
        {
            return EndAccountSession(null);
        }

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>true</returns>
        public virtual TwitterHashResponse EndAccountSession(Action<TwitterAsyncResponse<Account>> callback)
        {
            var accountUrl = BaseUrl + "account/end_session.json";

            var reqProc = new AccountRequestProcessor<Account>();

            TwitterExecutor.AsyncCallback = callback;
            var results =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            var acct = reqProc.ProcessActionResult(results, AccountAction.EndSession);

            if (acct != null)
            {
                return acct.EndSessionStatus;
            }
            throw new WebException("Unknown Twitter Response.");
        }
        
        /// <summary>
        /// Updates notification device for account
        /// </summary>
        /// <param name="device">type of device to use</param>
        /// <returns>User info</returns>
        // TODO: remove after deprecation date 
        [Obsolete("Deprecated as of 5/14/12.", true)]
        public virtual User UpdateAccountDeliveryDevice(DeviceType device)
        {
            return UpdateAccountDeliveryDevice(device, null);
        }

        /// <summary>
        /// Updates notification device for account
        /// </summary>
        /// <param name="device">type of device to use</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User info</returns>
        // TODO: remove after deprecation date 
        [Obsolete("Deprecated as of 5/14/12.", true)]
        public virtual User UpdateAccountDeliveryDevice(DeviceType device, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = BaseUrl + "account/update_delivery_device.xml";

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() }
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User info with new colors</returns>
        public virtual User UpdateAccountColors(string background, string text, string link, string sidebarFill, string sidebarBorder, Action<TwitterAsyncResponse<User>> callback)
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

            Debug.Assert(background != null, "background != null");
            Debug.Assert(text != null, "text != null");
            Debug.Assert(link != null, "link != null");
            Debug.Assert(sidebarFill != null, "sidebarFill != null");

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
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
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return UpdateAccountImage(imageFilePath, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace user image
        /// </summary>
        /// <remarks>
        /// You can only run this method with a period of time between executions; 
        /// otherwise you get WebException errors from Twitter
        /// </remarks>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountImage(string imageFilePath, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = BaseUrl + "account/update_profile_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.PostTwitterFile(accountUrl, null, imageFilePath, reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return UpdateAccountImage(image, fileName, imageType, null);
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
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountImage(byte[] image, string fileName, string imageType, Action<TwitterAsyncResponse<User>> callback)
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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.PostTwitterImage(accountUrl, null, image, fileName, imageType, reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="tile">Tile image in background</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountBackgroundImage(string imageFilePath, bool tile, bool use)
        {
            return UpdateAccountBackgroundImage(imageFilePath, tile, use, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <param name="tile">Tile image in background</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountBackgroundImage(string imageFilePath, bool tile, bool use, Action<TwitterAsyncResponse<User>> callback)
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
                    { "tile", true.ToString(CultureInfo.InvariantCulture).ToLower() },
                    { "use", use.ToString(CultureInfo.InvariantCulture).ToLower() }
                };
            }

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.PostTwitterFile(accountUrl, parameters, imageFilePath, reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return UpdateAccountBackgroundImage(image, fileName, imageType, tile, use, null);
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="image">full path to file, including file name</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
        /// <param name="tile">Tile image across background.</param>
        /// <param name="use">Whether to use uploaded background image or not</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new image info</returns>
        public virtual User UpdateAccountBackgroundImage(byte[] image, string fileName, string imageType, bool tile, bool use, Action<TwitterAsyncResponse<User>> callback)
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
                    { "tile", true.ToString(CultureInfo.InvariantCulture).ToLower() },
                    { "use", use.ToString(CultureInfo.InvariantCulture).ToLower() }
                };
            }

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.PostTwitterImage(accountUrl, parameters, image, fileName, imageType, reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
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
            return UpdateAccountProfile(name, url, location, description, null);
        }

        /// <summary>
        /// Update account profile info
        /// </summary>
        /// <param name="name">User Name</param>
        /// <param name="url">Web Address</param>
        /// <param name="location">Geographic Location</param>
        /// <param name="description">Personal Description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User with new info</returns>
        public virtual User UpdateAccountProfile(string name, string url, string location, string description, Action<TwitterAsyncResponse<User>> callback)
        {
            var accountUrl = BaseUrl + "account/update_profile.xml";

            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(url) &&
                string.IsNullOrEmpty(location) &&
                string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("At least one of the text fields (name, email, url, location, or description) must be provided as arguments, but none are specified.");
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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "name", name },
                        { "url", url },
                        { "location", location },
                        { "description", description }
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch CreateSavedSearch(string query)
        {
            return CreateSavedSearch(query, null);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch CreateSavedSearch(string query, Action<TwitterAsyncResponse<SavedSearch>> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query is required.", "query");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/create.xml";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    },
                    reqProc);

            List<SavedSearch> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch DestroySavedSearch(int id)
        {
            return DestroySavedSearch(id, null);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="id">ID of saved search</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public virtual SavedSearch DestroySavedSearch(int id, Action<TwitterAsyncResponse<SavedSearch>> callback)
        {
            if (id < 1)
            {
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/destroy/" + id + ".xml";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<SavedSearch> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user report spam
        /// </summary>
        /// <param name="id">id of alleged spammer</param>
        /// <param name="userID">user id of alleged spammer</param>
        /// <param name="screenName">screen name of alleged spammer</param>
        /// <returns>Alleged spammer user info</returns>
        public virtual User ReportSpam(string id, string userID, string screenName)
        {
            return ReportSpam(id, userID, screenName, null);
        }

        /// <summary>
        /// lets logged-in user report spam
        /// </summary>
        /// <param name="id">id of alleged spammer</param>
        /// <param name="userID">user id of alleged spammer</param>
        /// <param name="screenName">screen name of alleged spammer</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Alleged spammer user info</returns>
        public virtual User ReportSpam(string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
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

            var reqProc = new UserRequestProcessor<User>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    reportSpamUrl,
                    createParams,
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status Retweet(string id)
        {
            return Retweet(id, null);
        }

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public virtual Status Retweet(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var retweetUrl = BaseUrl + "statuses/retweet/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    retweetUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new list
        /// </summary>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for new list</returns>
        public virtual List CreateList(string listName, string mode, string description)
        {
            return CreateList(listName, mode, description, null);
        }

        /// <summary>
        /// Creates a new list
        /// </summary>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for new list</returns>
        public virtual List CreateList(string listName, string mode, string description, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw new ArgumentException("listName is required.", "listName");
            }

            var createUrl = BaseUrl + "lists/create.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    createUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Modifies an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for modified list</returns>
        public virtual List UpdateList(string listID, string slug, string ownerID, string ownerScreenName, string mode, string description)
        {
            return UpdateList(listID, slug, ownerID, ownerScreenName, mode, description, null);
        }

        /// <summary>
        /// Modifies an existing list
        /// </summary>
        /// <param name="listID">ID of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for modified list</returns>
        public virtual List UpdateList(string listID, string slug, string ownerID, string ownerScreenName, string mode, string description, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var updateListUrl = BaseUrl + "lists/update.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    updateListUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "mode", mode },
                        { "description", description }
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for deleted list</returns>
        public virtual List DeleteList(string listID, string slug, string ownerID, string ownerScreenName)
        {
            return DeleteList(listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for deleted list</returns>
        public virtual List DeleteList(string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("listID is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var deleteUrl = BaseUrl + "lists/destroy.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName }
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Adds a user as a list member.
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member added to.</returns>
        public virtual List AddMemberToList(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return AddMemberToList(userID, screenName, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Adds a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list member added to</returns>
        public virtual List AddMemberToList(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(userID) && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);
            }

            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var addMemberUrl = BaseUrl + "lists/members/create.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    addMemberUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="screenNames">List of user screen names to be list members.</param>
        /// <returns>List info for list members added to.</returns>
        public virtual List AddMemberRangeToList(string listID, string slug, string ownerID, string ownerScreenName, List<string> screenNames)
        {
            return AddMemberRangeToList(listID, slug, ownerID, ownerScreenName, screenNames, null);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="screenNames">List of user screen names to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        public virtual List AddMemberRangeToList(string listID, string slug, string ownerID, string ownerScreenName, List<string> screenNames, Action<TwitterAsyncResponse<List>> callback)
        {
            if (screenNames == null || screenNames.Count == 0)
            {
                throw new ArgumentException("screenNames is required. Check to see if the argument is null or the List<string> is empty.", "screenNames");
            }

            if (screenNames != null && screenNames.Count > 100)
            {
                throw new ArgumentException("Max screenNames is 100 at a time.", "screenNames");
            }

            return AddMemberRangeToList(listID, slug, ownerID, ownerScreenName, null, screenNames, callback);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <returns>List info for list members added to.</returns>
        public virtual List AddMemberRangeToList(string listID, string slug, string ownerID, string ownerScreenName, List<ulong> userIDs)
        {
            return AddMemberRangeToList(listID, slug, ownerID, ownerScreenName, userIDs, null);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        public virtual List AddMemberRangeToList(string listID, string slug, string ownerID, string ownerScreenName, List<ulong> userIDs, Action<TwitterAsyncResponse<List>> callback)
        {
            if (userIDs == null || userIDs.Count == 0)
            {
                throw new ArgumentException("userIDs is required. Check to see if the argument is null or the List<ulong> is empty.", "userIDs");
            }

            if (userIDs != null && userIDs.Count > 100)
            {
                throw new ArgumentException("Max user IDs is 100 at a time.", "userIDs");
            }

            return AddMemberRangeToList(listID, slug, ownerID, ownerScreenName, userIDs, null, callback);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <param name="screenNames">List of user screen names to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        private List AddMemberRangeToList(string listID, string slug, string ownerID, string ownerScreenName, IEnumerable<ulong> userIDs, List<string> screenNames, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }
            
            var addMemberRangeUrl = BaseUrl + "lists/members/create_all.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    addMemberRangeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "user_id", userIDs == null ? null : string.Join(",", userIDs.Select(id => id.ToString(CultureInfo.InvariantCulture)).ToArray()) },                        
                        { "screen_name", screenNames == null ? null : string.Join(",", screenNames.ToArray()) }
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Removes a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member removed from</returns>
        public virtual List DeleteMemberFromList(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return DeleteMemberFromList(userID, screenName, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Removes a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async callback</param>
        /// <returns>List info for list member removed from</returns>
        public virtual List DeleteMemberFromList(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(userID) && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);
            }

            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var deleteUrl = BaseUrl + "lists/members/destroy.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
        
        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscribed to</returns>
        public virtual List SubscribeToList(string listID, string slug, string ownerID, string ownerScreenName)
        {
            return SubscribeToList(listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list subscribed to</returns>
        public virtual List SubscribeToList(string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var subscribeUrl = BaseUrl + "lists/subscribers/create.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    subscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public virtual List UnsubscribeFromList(string listID, string slug, string ownerID, string ownerScreenName)
        {
            return UnsubscribeFromList(listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list subscription removed from</returns>
        public virtual List UnsubscribeFromList(string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var unsubscribeUrl = BaseUrl + "lists/subscribers/destroy.xml";

            var reqProc = new ListRequestProcessor<List>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    unsubscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List<List> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Lets you perform a query by specifying the raw URL and parameters yourself.
        /// Useful for when Twitter changes or adds new features before they are added to LINQ to Twitter.
        /// </summary>
        /// <param name="queryString">The segments that follow the base URL. i.e. "statuses/public_timeline.xml" for a public status query</param>
        /// <param name="parameters">Querystring parameters that will be appended to the URL</param>
        /// <returns></returns>
        public string ExecuteRaw(string queryString, Dictionary<string, string> parameters)
        {
            return ExecuteRaw(queryString, parameters, null);
        }

        /// <summary>
        /// Lets you perform a query by specifying the raw URL and parameters yourself.
        /// Useful for when Twitter changes or adds new features before they are added to LINQ to Twitter.
        /// </summary>
        /// <param name="queryString">The segments that follow the base URL. i.e. "statuses/public_timeline.xml" for a public status query</param>
        /// <param name="parameters">Querystring parameters that will be appended to the URL</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns></returns>
        public string ExecuteRaw(string queryString, Dictionary<string, string> parameters, Action<TwitterAsyncResponse<string>> callback)
        {
            string rawUrl = BaseUrl.TrimEnd('/') + "/" + queryString.TrimStart('/');

            var reqProc = new RawRequestProcessor<Raw>();

            TwitterExecutor.AsyncCallback = callback;
            var resultsXml =
                TwitterExecutor.ExecuteTwitter(
                    rawUrl,
                    parameters,
                    reqProc);

            return resultsXml;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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
                var disposableExecutor = TwitterExecutor as IDisposable;
                if (disposableExecutor != null)
                {
                    disposableExecutor.Dispose();
                }
            }
        }
    }
}