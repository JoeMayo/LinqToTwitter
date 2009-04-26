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

namespace LinqToTwitter
{
    /// <summary>
    /// manages access to Twitter API
    /// </summary>
    public class TwitterContext
    {
        #region TwitterContext initialization

        /// <summary>
        /// login name of user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// user's password
        /// </summary>
        public string Password { get; set; }

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
            UserName = userName;
            Password = password;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? "http://twitter.com/" : baseUrl;
            SearchUrl = string.IsNullOrEmpty(searchUrl) ? "http://search.twitter.com/" : searchUrl;
        }

        #endregion

        #region TwitterQueryable objects

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
            var queryableList = QueryTwitter(url, reqProc);

            return queryableList;
        }

        /// <summary>
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        public IRequestProcessor CreateRequestProcessor(Expression expression)
        {
            var requestType = expression.Type.GetGenericArguments()[0].Name;

            IRequestProcessor req;

            switch (requestType)
            {
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

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        private IQueryable QueryTwitter(string url, IRequestProcessor requestProcessor)
        {
            var req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Credentials = new NetworkCredential(UserName, Password);
            req.UserAgent = "LINQ to Twitter";

            var resp = req.GetResponse();

            StringReader txtRdr;

            using (var strm = resp.GetResponseStream())
            {
                var strmRdr = new StreamReader(strm);
                txtRdr = new StringReader(strmRdr.ReadToEnd());
            }

            var statusXml = XElement.Load(txtRdr);

            var results = requestProcessor.ProcessResults(statusXml);
            return results;
        }

        #endregion

        #region Twitter Execution API

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        private IQueryable ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            var req = WebRequest.Create(url) as HttpWebRequest;
            req.Credentials = new NetworkCredential(UserName, Password);
            req.Method = "POST";
            var paramsJoined =
                string.Join(
                    "&",
                    (from param in parameters
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + HttpUtility.UrlEncode(param.Value))
                     .ToArray());
            var bytes = Encoding.UTF8.GetBytes(paramsJoined);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bytes.Length;
            req.UserAgent = "LINQ to Twitter";

            // due to Twitter API change, I needed to remove the Expect header. More details here by Phil Haack - Joe
            // http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
            req.ServicePoint.Expect100Continue = false;

            string responseXML;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);

                var resp = req.GetResponse();

                using (var respStream = resp.GetResponseStream())
                using (var respRdr = new StreamReader(respStream))
                {
                    responseXML = respRdr.ReadToEnd();
                }
            }

            var responseXElem = XElement.Parse(responseXML);
            var results = requestProcessor.ProcessResults(responseXElem);
            return results;
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public IQueryable<Status> UpdateStatus(string status)
        {
            return UpdateStatus(status, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public IQueryable<Status> UpdateStatus(string status, string inReplyToStatusID)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("status is a required parameter.");
            }

            status = status.Substring(0, Math.Min(140, status.Length));

            var updateUrl = BaseUrl + "statuses/update.xml";

            var results =
                ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID}
                    },
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
        }

        // TODO: Remove Destroy at v1.0 - Joe

        /// <summary>
        /// deletes a tweet
        /// </summary>
        /// <param name="id">id of tweet</param>
        /// <returns>deleted tweet</returns>
        [Obsolete("Destroy is on the fast track to deprecation.  Please use DestroyStatus instead, which is more descriptive and consistent with other DestroyXxx methods. Thanks for using LINQ to Twitter - Joe :)")]
        public IQueryable<Status> Destroy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
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
        public IQueryable<Status> DestroyStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
        }

        /// <summary>
        /// sends a new direct message to specified userr
        /// </summary>
        /// <param name="userID">id of user to send to</param>
        /// <param name="id">text to send</param>
        /// <returns>direct message element</returns>
        public IQueryable<DirectMessage> NewDirectMessage(string userID, string text)
        {
            var newUrl = BaseUrl + "direct_messages/new.xml";

            var results =
                ExecuteTwitter(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", userID},
                        {"text", text}
                    },
                    new DirectMessageRequestProcessor());

            return results as IQueryable<DirectMessage>;
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="id">id of direct message</param>
        /// <returns>direct message element</returns>
        public IQueryable<DirectMessage> DestroyDirectMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "direct_messages/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new DirectMessageRequestProcessor());

            return results as IQueryable<DirectMessage>;
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public IQueryable<User> CreateFriendship(string id, bool follow)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "friendships/create/" + id + ".xml";

            Dictionary<string, string> createParams = new Dictionary<string, string>();
            
            // If follow exists in the parameter list, Twitter will
            // always treat it as true, even if the value is false;
            // Therefore, only add follow if it is true.
            if (follow)
            {
                createParams.Add("follow", "true");
            }

            var results =
                ExecuteTwitter(
                    destroyUrl,
                    createParams,
                    new UserRequestProcessor());

            return results as IQueryable<User>;
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">id of user to follow</param>
        /// <returns>followed friend user info</returns>
        public IQueryable<User> DestroyFriendship(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "friendships/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new UserRequestProcessor());

            return results as IQueryable<User>;
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public IQueryable<Status> CreateFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/create/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public IQueryable<Status> DestroyFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = BaseUrl + "favorites/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
        }

        /// <summary>
        /// Disables notifications from specified user
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to disable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public IQueryable<User> DisableNotifications(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            var notificationsUrl = BaseUrl + "notifications/leave/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return results as IQueryable<User>;
        }

        /// <summary>
        /// Enables notifications from specified user
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to enable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public IQueryable<User> EnableNotifications(string id, string userID, string screenName)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            var notificationsUrl = BaseUrl + "notifications/follow/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    new UserRequestProcessor());

            return results as IQueryable<User>;
        }

        #endregion
    }
}