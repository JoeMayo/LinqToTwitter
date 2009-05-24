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

namespace LinqToTwitter
{
    /// <summary>
    /// manages access to Twitter API
    /// </summary>
    public class TwitterContext
    {
        #region TwitterContext initialization

        // TODO: Add support for response headers, i.e. X-RateLimit-Reset - Joe
 
        // TODO: Remove Since parameters, which have been removed from the API. - Joe

        /// <summary>
        /// base URL for accessing Twitter API
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// base URL for accessing Twitter Search API
        /// </summary>
        public string SearchUrl { get; set; }

        /// <summary>
        /// default constructor, results in no credentials and BaseUrl set to http://twitter.com/
        /// </summary>
        public TwitterContext() :
            this(string.Empty, string.Empty, string.Empty, string.Empty) { }

        /// <summary>
        /// Call this constructor for IoC testing
        /// </summary>
        public TwitterContext(ITwitterExecute twitterExecute) :
            this(string.Empty, string.Empty, string.Empty, string.Empty) 
        {
            TwitterExecute = twitterExecute;
        }

        /// <summary>
        /// initializes TwitterContext with username and password - BaseUrl defaults to http://twitter.com/
        /// </summary>
        /// <param name="userName">name of user</param>
        /// <param name="password">user's password</param>
        public TwitterContext(string userName, string password) :
            this(userName, password, string.Empty, string.Empty) { }

        /// <summary>
        /// initializes TwitterContext with username and password - BaseUrl defaults to http://twitter.com/
        /// </summary>
        /// <param name="userName">name of user</param>
        /// <param name="password">user's password</param>
        /// <param name="baseUrl">base url of Twitter API</param>
        public TwitterContext(string userName, string password, string baseUrl) :
            this(userName, password, baseUrl, string.Empty) { }

        /// <summary>
        /// initialize TwitterContext with credentials and custom BaseUrl
        /// </summary>
        /// <param name="userName">name of user</param>
        /// <param name="password">user's password</param>
        /// <param name="baseUrl">base url of Twitter API</param>
        /// <param name="searchUrl">base url of Twitter Search API</param>
        public TwitterContext(string userName, string password, string baseUrl, string searchUrl)
        {
            TwitterExecute = new TwitterExecute();
            TwitterExecute.OAuthTwitter = OAuthTwitter;
            UserName = userName;
            Password = password;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "http://twitter.com/" : baseUrl;
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "http://search.twitter.com/" : searchUrl;
            OAuthAccessTokenUrl = "http://twitter.com/oauth/access_token";
            OAuthAuthorizeUrl = "http://twitter.com/oauth/authorize";
            OAuthRequestTokenUrl = "http://twitter.com/oauth/request_token";
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
        /// login name of user
        /// </summary>
        public string UserName
        {
            get
            {
                if (TwitterExecute != null)
                {
                    return TwitterExecute.UserName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (TwitterExecute != null)
                {
                    TwitterExecute.UserName = value;
                }
            }
        }

        /// <summary>
        /// user's password
        /// </summary>
        public string Password
        {
            get
            {
                if (TwitterExecute != null)
                {
                    return TwitterExecute.Password;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (TwitterExecute != null)
                {
                    TwitterExecute.Password = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                if (TwitterExecute != null)
                {
                    return TwitterExecute.UserAgent;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (TwitterExecute != null)
                {
                    TwitterExecute.UserAgent = value;
                }
            }
        }

        /// <summary>
        /// Methods for communicating with Twitter
        /// </summary>
        private ITwitterExecute TwitterExecute { get; set; }

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

        #region OAuth Support

        // TODO: check support for uploading images via https - Joe 

        /// <summary>
        /// OAuth Consumer key - must be set for OAuth calls.
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                if (OAuthTwitter != null)
                {
                    return OAuthTwitter.OAuthConsumerKey;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (OAuthTwitter != null)
                {
                    OAuthTwitter.OAuthConsumerKey = value;
                }
            }
        }

        /// <summary>
        /// OAuth Consumer secret - must be set for OAuth calls.
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                if (OAuthTwitter != null)
                {
                    return OAuthTwitter.OAuthConsumerSecret;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (OAuthTwitter != null)
                {
                    OAuthTwitter.OAuthConsumerSecret = value;
                }
            }
        }

        /// <summary>
        /// URL for OAuth Request Tokens
        /// </summary>
        public string OAuthRequestTokenUrl { get; set; }

        /// <summary>
        /// URL for OAuth authorization
        /// </summary>
        public string OAuthAuthorizeUrl { get; set; }

        /// <summary>
        /// URL for OAuth Access Tokens
        /// </summary>
        public string OAuthAccessTokenUrl { get; set; }

        /// <summary>
        /// Screen name returned from OAuth Token Request
        /// </summary>
        public string OAuthRequestScreenName { get; set; }

        /// <summary>
        /// User ID returned from OAuth Token Request
        /// </summary>
        public string OAuthRequestUserID { get; set; }

        /// <summary>
        /// Backing store for OAuthTwitter instance
        /// </summary>
        private OAuthTwitter m_oAuthTwitter = null;

        /// <summary>
        /// reference to OAuthTwitter for authentication
        /// </summary>
        private OAuthTwitter OAuthTwitter 
        { 
            get
            {
                if (m_oAuthTwitter == null)
                {
                    m_oAuthTwitter = new OAuthTwitter
                    {
                        OAuthUserAgent = UserAgent
                    };
                }

                return m_oAuthTwitter;
            }
        }

        /// <summary>
        /// True if OAuth succeeds, otherwise false.
        /// </summary>
        public bool AuthorizedViaOAuth
        {
            get
            {
                return !string.IsNullOrEmpty(OAuthTwitter.OAuthTokenSecret);
            }
        }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <param name="readOnly">true for read-only, otherwise read/Write</param>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string GetAuthorizationPageLink(bool readOnly)
        {
            return OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl, readOnly);
        }

        /// <summary>
        /// Retrieves access token from Twitter.
        /// Call this after calling GetAuthorizationPageLink()
        /// </summary>
        /// <param name="oAuthToken">Auth Token from call to GetAuthorizationPageLink</param>
        public void RetrieveAccessToken(string oAuthToken)
        {
            if (string.IsNullOrEmpty(oAuthToken))
            {
                throw new ArgumentException("Invalid OAuth Token.", "oAuthToken");
            }

            string screenName;
            string userID;

            OAuthTwitter.AccessTokenGet(oAuthToken, OAuthAccessTokenUrl, out screenName, out userID);

            OAuthRequestScreenName = screenName;
            OAuthRequestUserID = userID;
        }

        #endregion

        #region Twitter Query API

        /// <summary>
        /// Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <returns>list of objects with query results</returns>
        internal IQueryable Execute(Expression expression)
        {
            Dictionary<string, string> parameters = null;

            // request processor is specific to request type (i.e. Status, User, etc.)
            var reqProc = CreateRequestProcessor(expression);

            // we need the where expression because it contains the criteria for the request
            var whereFinder = new FirstWhereClauseFinder();
            var whereExpression = whereFinder.GetFirstWhere(expression);

            if (whereExpression != null)
            {
                var lambdaExpression = (LambdaExpression) 
                    ((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                // translate variable references in expression into constants
                lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                parameters = reqProc.GetParameters(lambdaExpression);
            }

            // construct REST endpoint, based on input parameters
            var url = reqProc.BuildURL(parameters);

            // execute the query and return results
            var queryableList = TwitterExecute.QueryTwitter(url, reqProc);

            return queryableList;
        }

        /// <summary>
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        private IRequestProcessor CreateRequestProcessor(Expression expression)
        {
            var requestType = expression.Type.GetGenericArguments()[0].Name;

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
                TwitterExecute.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID}
                    },
                    new StatusRequestProcessor());

            return (results as IQueryable<Status>).FirstOrDefault();
        }

        // TODO: Remove Destroy at v1.0 - Joe

        /// <summary>
        /// deletes a tweet
        /// </summary>
        /// <param name="id">id of tweet</param>
        /// <returns>deleted tweet</returns>
        [Obsolete("Destroy is on the fast track to deprecation.  Please use DestroyStatus instead, which is more descriptive and consistent with other DestroyXxx methods. Thanks for using LINQ to Twitter - Joe :)", true)]
        public IQueryable<Status> Destroy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
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
                TwitterExecute.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IQueryable<Status>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", userID},
                        {"text", text}
                    },
                    new DirectMessageRequestProcessor());

            return (results as IQueryable<DirectMessage>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new DirectMessageRequestProcessor());

            return (results as IQueryable<DirectMessage>).FirstOrDefault();
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

            var destroyUrl = BaseUrl + "friendships/create/" + id + ".xml";

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
                TwitterExecute.ExecuteTwitter(
                    destroyUrl,
                    createParams,
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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

            var destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    },
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IQueryable<Status>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return (results as IQueryable<Status>).FirstOrDefault();
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

            var notificationsUrl = BaseUrl + "notifications/leave/" + id + ".xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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

            var notificationsUrl = BaseUrl + "notifications/follow/" + id + ".xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
        }

        /// <summary>
        /// sends a test message to twitter to check connectivity
        /// </summary>
        /// <returns>true</returns>
        public bool HelpTest()
        {
            var helpUrl = BaseUrl + "help/test.xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    helpUrl,
                    new Dictionary<string, string>(),
                    new HelpRequestProcessor());

            return (results as IQueryable<bool>).FirstOrDefault();
        }

        /// <summary>
        /// Ends the session for the currently logged in user
        /// </summary>
        /// <returns>true</returns>
        public TwitterHashResponse EndAccountSession()
        {
            var accountUrl = BaseUrl + "account/end_session.xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>(),
                    new AccountRequestProcessor());

            var acct = (results as IQueryable<Account>).FirstOrDefault();

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
            // TODO: Follow-up on this API; It always returns false and Twitter doesn't seem to be working the same as the API docs - Joe
            var accountUrl = BaseUrl + "account/update_delivery_device.xml";

            var results =
                TwitterExecute.ExecuteTwitter(
                    accountUrl,
                    new Dictionary<string, string>
                    {
                        { "device", device.ToString().ToLower() }
                    },
                    new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
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

            return (results as IQueryable<User>).FirstOrDefault();
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

            var results = TwitterExecute.PostTwitterFile(imageFilePath, accountUrl, new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
        }

        /// <summary>
        /// sends an image file to Twitter to replace background image
        /// </summary>
        /// <param name="imageFilePath">full path to file, including file name</param>
        /// <returns>User with new image info</returns>
        public User UpdateAccountBackgroundImage(string imageFilePath, bool tile)
        {
            // TODO: finish tile implementation - Joe

            var accountUrl = BaseUrl + "account/update_profile_background_image.xml";

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("imageFilePath is required.", "imageFilePath");
            }

            var results = TwitterExecute.PostTwitterFile(imageFilePath, accountUrl, new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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
                TwitterExecute.ExecuteTwitter(
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

            return (results as IQueryable<User>).FirstOrDefault();
        }

        #endregion
    }
}