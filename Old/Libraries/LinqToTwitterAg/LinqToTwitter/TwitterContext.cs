/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 ***********************************************************/
using System;
using System.Collections.Generic;
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
        // header constants
        //

        public const string XRateLimitLimitKey = "x-rate-limit-limit";
        public const string XRateLimitRemainingKey = "x-rate-limit-remaining";
        public const string XRateLimitResetKey = "x-rate-limit-reset";
        public const string RetryAfterKey = "Retry-After";
        public const string XMediaRateLimitLimitKey = "x-mediaratelimit-limit";
        public const string XMediaRateLimitRemainingKey = "x-mediaratelimit-remaining";
        public const string XMediaRateLimitResetKey = "x-mediaratelimit-reset";
        public const string DateKey = "Date";

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        [Obsolete("Twitter API v1.1 requires all queries to be authorized. Please visit http://linqtotwitter.codeplex.com/wikipage?title=Securing%20Your%20Applications for more guidance on how to use OAuth in your application.", true)]
        public TwitterContext()
            : this(new AnonymousAuthorizer())
        {
            BaseUrl = "https://api.twitter.com/1.1/";
            SearchUrl = "https://api.twitter.com/1.1/search/";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterAuthorizer authorization)
#if SILVERLIGHT
            : this(authorization, (IWebRequestCreate)null)
#else
            : this(new TwitterExecute(authorization))
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="baseUrl">Overwrites default base URL</param>
        /// <param name="searchUrl">Overwrites default search URL</param>
        [Obsolete("The Search URL is now the same as the Base URL in Twitter API v1.1.", true)]
        public TwitterContext(ITwitterAuthorizer authorization, string baseUrl, string searchUrl)
            : this(new TwitterExecute(authorization))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="execute">The <see cref="ITwitterExecute"/> object to use.</param>
        public TwitterContext(ITwitterExecute execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            TwitterExecutor = execute;
            BaseUrl = "https://api.twitter.com/1.1/";
            SearchUrl = "https://api.twitter.com/1.1/search/";
            StreamingUrl = "https://stream.twitter.com/1.1/";
            UserStreamUrl = "https://userstream.twitter.com/1.1/";
            SiteStreamUrl = "https://sitestream.twitter.com/1.1/";
        }


#if SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterContext"/> class.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        public TwitterContext(ITwitterAuthorizer authorization, IWebRequestCreate webReqCreateInput)
            : this(new TwitterExecute(authorization))
        {
            IWebRequestCreate webReqCreator = webReqCreateInput;
                
            if (webReqCreateInput == null)
                webReqCreator =WebRequestCreator.ClientHttp;
#if !WINDOWS_PHONE
            if (webReqCreateInput == null && 
                !System.Windows.Application.Current.IsRunningOutOfBrowser)
                webReqCreator = WebRequestCreator.BrowserHttp;
#endif
            WebRequest.RegisterPrefix("http://", webReqCreator);
            WebRequest.RegisterPrefix("https://", webReqCreator);
        }
#endif

        /// <summary>
        /// Gets the screen name of the user (only populated when a request for access token occurs)
        /// </summary>
        public string UserName
        {
            get { return AuthorizedClient.ScreenName; }
        }

        /// <summary>
        /// base URL for accessing Twitter API
        /// </summary>
        public string BaseUrl { get; set; }

        // TODO: BaseUrl and SearchUrl are the same in Twitter API v1.1. Remove SearchUrl and refactor as necessary.
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
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        [Obsolete("Please use OAuth instead.", true)]
        public string StreamingUserName 
        {
            get { return TwitterExecutor.StreamingUserName; }
            set { TwitterExecutor.StreamingUserName = value; }
        }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        [Obsolete("Please use OAuth instead.", true)]
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

        /// <summary>
        /// This contains the JSON string from the Twitter response to the most recent query.
        /// </summary>
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

#if !SILVERLIGHT && !NETFX_CORE
        /// <summary>
        /// Proxy for all HTTP communications
        /// </summary>
        public WebProxy Proxy
        {
            get { return AuthorizedClient.Proxy; }
            set { AuthorizedClient.Proxy = value; }
        }
#endif

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
        /// enables access to Twitter Control Streams, which manage and query Site Streams
        /// </summary>
        public TwitterQueryable<ControlStream> ControlStream
        {
            get
            {
                return new TwitterQueryable<ControlStream>(this);
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
        /// enables access to Twitter Search to query tweets
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
        public IDictionary<string, string> ResponseHeaders
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
        /// Max number of requests per window for
        /// TweetWithMediaAsync and ReplyWithMediaAsync.
        /// </summary>
        public int MediaRateLimitCurrent
        {
            get
            {
                return GetResponseHeaderAsInt(XMediaRateLimitLimitKey);
            }
        }

        /// <summary>
        /// Number of requests available until reset
        /// for TweetWithMediaAsync and ReplyWithMediaAsync.
        /// </summary>
        public int MediaRateLimitRemaining
        {
            get
            {
                return GetResponseHeaderAsInt(XMediaRateLimitRemainingKey);
            }
        }

        /// <summary>
        /// UTC time in ticks until rate limit resets
        /// for TweetWithMediaAsync and ReplyWithMediaAsync.
        /// </summary>
        public int MediaRateLimitReset
        {
            get
            {
                return GetResponseHeaderAsInt(XMediaRateLimitResetKey);
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
            var request = reqProc.BuildUrl(parameters);

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
            var parameters = new Dictionary<string, string>();

            // GHK FIX: Handle all wheres
            var whereExpressions = new WhereClauseFinder().GetAllWheres(expression);
            foreach (var whereExpression in whereExpressions)
            {
                var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                // translate variable references in expression into constants
                lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                var newParameters = reqProc.GetParameters(lambdaExpression);
                foreach (var newParameter in newParameters)
                {
                    if (!parameters.ContainsKey(newParameter.Key))
                    {
                        parameters.Add(newParameter.Key, newParameter.Value);
                    }
                }
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
                const string NullExpressionMessage = "Expression passed to CreateRequestProcessor must not be null.";
                throw new ArgumentNullException("Expression", NullExpressionMessage);
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
                case "ControlStream":
                    req = new ControlStreamRequestProcessor<T>
                        {
                            SiteStreamUrl = SiteStreamUrl
                        };
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