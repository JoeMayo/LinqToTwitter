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

        private string m_userAgent = "LINQ To Twitter v1.0";

        /// <summary>
        /// User agent for queries
        /// </summary>
        public string UserAgent
        {
            get
            {
                return m_userAgent;
            }
            set
            {
                m_userAgent = 
                    string.IsNullOrEmpty(value) ? 
                        m_userAgent : 
                        value + ";" + m_userAgent;
            }
        }

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
            OAuthAccessTokenUrl = "http://twitter.com/oauth/access_token";
            OAuthAuthorizeUrl = "http://twitter.com/oauth/authorize";
            OAuthRequestTokenUrl = "http://twitter.com/oauth/request_token";
        }

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

        /// <summary>
        /// OAuth Consumer key - must be set for OAuth calls.
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// OAuth Consumer secret - must be set for OAuth calls.
        /// </summary>
        public string ConsumerSecret { get; set; }

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
                        OAuthUserAgent = UserAgent,
                        OAuthConsumerKey = ConsumerKey,
                        OAuthConsumerSecret = ConsumerSecret
                    };
                }

                return m_oAuthTwitter;
            }
        }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string GetAuthorizationPageLink()
        {
            return OAuthTwitter.AuthorizationLinkGet(OAuthRequestTokenUrl, OAuthAuthorizeUrl);
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

            OAuthTwitter.AccessTokenGet(oAuthToken, OAuthAccessTokenUrl);
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

        #endregion

        #region Execution and Query Helper Methods

        /// <summary>
        /// generates a new TwitterQueryException from a WebException
        /// </summary>
        /// <param name="wex">Web Exception to Translate</param>
        /// <returns>new TwitterQueryException instance</returns>
        private TwitterQueryException CreateTwitterQueryException(WebException wex)
        {
            var responseStr = GetTwitterResponse(wex.Response);

            XElement responseXml;

            try
            {
                responseXml = XElement.Parse(responseStr);
            }
            catch (Exception)
            {
                // One known reason this can happen is if you don't have an 
                // Internet connection, meaning that the response will contain
                // an HTML message, that can't be parsed as normal XML.
                responseXml = XElement.Parse(
@"<hash>
  <request>" + wex.Response.ResponseUri + @"</request>
  <error>See Inner Exception Details for more information.</error>
</hash>");
            }

            return new TwitterQueryException("Error while querying Twitter.", wex)
            {
                HttpError = wex.Response.Headers["Status"],
                Response = new TwitterHashResponse
                {
                    Request = responseXml.Element("request").Value,
                    Error = responseXml.Element("error").Value
                }
            };
        }

        /// <summary>
        /// gets WebResponse contents from Twitter
        /// </summary>
        /// <param name="resp">WebResponse to extract string from</param>
        /// <returns>XML string response from Twitter</returns>
        private string GetTwitterResponse(WebResponse resp)
        {
            StringReader txtRdr;

            using (var strm = resp.GetResponseStream())
            {
                var strmRdr = new StreamReader(strm);
                txtRdr = new StringReader(strmRdr.ReadToEnd());
            }

            return txtRdr.ReadToEnd();
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
                case "Account":
                    req = new AccountRequestProcessor() { BaseUrl = BaseUrl };
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

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        private IQueryable QueryTwitter(string url, IRequestProcessor requestProcessor)
        {
            if (AuthorizedViaOAuth)
            {
                string outUrl;
                string queryString;
                OAuthTwitter.GetOAuthQueryString(HttpMethod.GET, url, out outUrl, out queryString);
                url = outUrl + "?" + queryString;
            }

            var req = HttpWebRequest.Create(url) as HttpWebRequest;

            if (!AuthorizedViaOAuth)
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            req.UserAgent = UserAgent;

            WebResponse resp = null;
            string responseStr = null;

            try
            {
                resp = req.GetResponse();
                responseStr = GetTwitterResponse(resp);
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close(); 
                }
            }

            XElement statusXml = null;

            if (new Uri(url).LocalPath.EndsWith("json"))
            {
                var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(responseStr));
                XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);

                var doc = new XmlDocument();
                doc.Load(reader);

                statusXml = XElement.Parse(doc.OuterXml);
            }
            else
            {
                statusXml = XElement.Parse(responseStr);
            }

            return requestProcessor.ProcessResults(statusXml);
        }

        #endregion

        #region Twitter Execution API

        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="fileName">name of file to upload</param>
        /// <param name="url">url to upload to</param>
        /// <returns>IQueryable</returns>
        private IQueryable PostTwitterFile(string filePath, string url, IRequestProcessor requestProcessor)
        {
            var file = Path.GetFileName(filePath);

            string imageType;

            switch (Path.GetExtension(file).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    imageType = "jpg";
                    break;
                case ".gif":
                    imageType = "gif";
                    break;
                case ".png":
                    imageType = "png";
                    break;
                default:
                    throw new ArgumentException(
                        "Can't recognize the extension of the file you're uploading. Please choose either a *.gif, *.jpg, *.jpeg, or *.png file.", filePath);
            }

            string contentBoundaryBase = DateTime.Now.Ticks.ToString("x");
            string beginContentBoundary = string.Format("--{0}\r\n", contentBoundaryBase);
            var contentDisposition = string.Format("Content-Disposition:form-data); name=\"image\"); filename=\"{0}\"\r\nContent-Type: image/{1}\r\n\r\n", file, imageType);
            var endContentBoundary = string.Format("\r\n--{0}--\r\n", contentBoundaryBase);

            byte[] fileBytes = null;
            string fileByteString = null;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4096];
                var memStr = new MemoryStream();
                memStr.Position = 0;
                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStr.Write(buffer, 0, bytesRead);
                }

                memStr.Position = 0;
                fileByteString = Encoding.GetEncoding("iso-8859-1").GetString(memStr.GetBuffer());
            }

            fileBytes = 
                Encoding.GetEncoding("iso-8859-1").GetBytes(
                    beginContentBoundary + 
                    contentDisposition + 
                    fileByteString + 
                    endContentBoundary);

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "multipart/form-data;boundary=" + contentBoundaryBase;
            req.PreAuthenticate = true;
            req.AllowWriteStreamBuffering = true;
            req.Method = "POST";
            req.UserAgent = UserAgent;
            req.ContentLength = fileBytes.Length;

            if (AuthorizedViaOAuth)
            {
                req.Headers.Add(
                    HttpRequestHeader.Authorization,
                    OAuthTwitter.GetOAuthAuthorizationHeader(url, null));
            }
            else
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            string responseXML = null;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(fileBytes, 0, fileBytes.Length);
                reqStream.Flush();
            }

            WebResponse resp = null;

            try
            {
                resp = req.GetResponse();

                using (var respStream = resp.GetResponseStream())
                using (var respRdr = new StreamReader(respStream))
                {
                    responseXML = respRdr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                var twitterQueryEx = CreateTwitterQueryException(wex);
                throw twitterQueryEx;
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }

            var responseXElem = XElement.Parse(responseXML);
            var results = requestProcessor.ProcessResults(responseXElem);
            return results;
        }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        private IQueryable ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            string paramsJoined = string.Empty;

            paramsJoined =
                string.Join(
                    "&",
                    (from param in parameters
                     where !string.IsNullOrEmpty(param.Value)
                     select param.Key + "=" + OAuthTwitter.OAuthParameterUrlEncode(param.Value))
                     .ToArray());

                url += "?" + paramsJoined;
            var req = WebRequest.Create(url) as HttpWebRequest;

            if (AuthorizedViaOAuth)
            {
                req.Headers.Add(
                    HttpRequestHeader.Authorization,
                    OAuthTwitter.GetOAuthAuthorizationHeader(url, null));
            }
            else
            {
                req.Credentials = new NetworkCredential(UserName, Password);
            }

            var bytes = Encoding.UTF8.GetBytes(paramsJoined);
            req.ContentLength = bytes.Length;
            req.Method = "POST";
            req.ContentType = "x-www-form-urlencoded";
            req.UserAgent = UserAgent;
            req.ServicePoint.Expect100Continue = false;

            string responseXML;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);

                WebResponse resp = null;

                try
                {
                    resp = req.GetResponse();

                    using (var respStream = resp.GetResponseStream())
                    using (var respRdr = new StreamReader(respStream))
                    {
                        responseXML = respRdr.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    var twitterQueryEx = CreateTwitterQueryException(wex);
                    throw twitterQueryEx;
                }
                finally
                {
                    if (resp != null)
                    {
                        resp.Close();
                    }
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
                ExecuteTwitter(
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
        public Status DestroyStatus(string id)
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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
                ExecuteTwitter(
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

            var results = PostTwitterFile(imageFilePath, accountUrl, new UserRequestProcessor());

            return (results as IQueryable<User>).FirstOrDefault();
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

            var results = PostTwitterFile(imageFilePath, accountUrl, new UserRequestProcessor());

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
                ExecuteTwitter(
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