/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace LinqToTwitter
{
    /// <summary>
    /// manages access to Twitter API
    /// </summary>
    public class TwitterContext
    {
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
        /// default constructor, results in no credentials and BaseUrl set to http://twitter.com/
        /// </summary>
        public TwitterContext() : 
            this(string.Empty, string.Empty, string.Empty) { }

        /// <summary>
        /// initializes TwitterContext with username and password - BaseUrl defaults to http://twitter.com/
        /// </summary>
        /// <param name="userName">name of user</param>
        /// <param name="password">user's password</param>
        public TwitterContext(string userName, string password) :
            this(userName, password, string.Empty) { }

        /// <summary>
        /// initialize TwitterContext with credentials and custom BaseUrl
        /// </summary>
        /// <param name="userName">name of user</param>
        /// <param name="password">user's password</param>
        /// <param name="baseUrl">base url of Twitter API</param>
        public TwitterContext(string userName, string password, string baseUrl)
        {
            UserName = userName;
            Password = password;
            BaseUrl = baseUrl == string.Empty ? "http://twitter.com/" : baseUrl;
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
        /// Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <returns>list of objects with query results</returns>
        internal object Execute(Expression expression)
        {
            Dictionary<string, string> parameters = null;

            var whereFinder = new InnermostWhereFinder();
            var whereExpression = whereFinder.GetInnermostWhere(expression);

            if (whereExpression != null)
            {
                var lambdaExpression = 
                    (LambdaExpression)
                    ((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                lambdaExpression = 
                    (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                var paramFinder =
                    new ParameterFinder<Status>(
                        lambdaExpression.Body,
                        new List<string> { "Type" });

                parameters = paramFinder.Parameters; 
            }

            //
            // here, we assume the results are for Status queries, 
            // but this will eventually be refactored to support more of the Twitter API
            //

            var statusList = GetStatusList(parameters);

            var queryableStatusList = statusList.AsQueryable<Status>();

            return queryableStatusList;
        }

        /// <summary>
        /// constructs the Twitter API URL and executes the query
        /// </summary>
        /// <param name="parameters">criteria for the query</param>
        /// <returns>List of Status objects</returns>
        private List<Status> GetStatusList(Dictionary<string, string> parameters)
        {
            var url = BuildUrl<Status>(parameters);
            var statusList = QueryTwitter(url);

            return statusList;
        }

        /// <summary>
        /// constructs an URL, based on type and parameter
        /// </summary>
        /// <typeparam name="T">type of query</typeparam>
        /// <param name="parameters">parameters for building URL</param>
        /// <returns>final URL to call Twitter API with</returns>
        private string BuildUrl<T>(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null ||
                !parameters.ContainsKey("Type"))
            {
                url = BaseUrl + "statuses/public_timeline.xml";
                return url;
            }

            switch (typeof(T).Name)
            {
                case "Status":
                    switch (parameters["Type"])
                    {
                        case "Public":
                            url = BaseUrl + "statuses/public_timeline.xml";
                            break;
                        case "Friends":
                            url = BaseUrl + "statuses/friends_timeline.xml";
                            break;
                        default:
                            url = BaseUrl + "statuses/public_timeline.xml";
                            break;
                    }
                    break;
                default:
                    url = BaseUrl + "statuses/public_timeline.xml";
                    break;
            }

            return url;
        }

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        private List<Status> QueryTwitter(string url)
        {
            var req = HttpWebRequest.Create(url);
            req.Credentials = new NetworkCredential(UserName, Password);
            var resp = req.GetResponse();
            var strm = resp.GetResponseStream();
            var strmRdr = new StreamReader(strm);
            var txtRdr = new StringReader(strmRdr.ReadToEnd());
            var statusXml = XElement.Load(txtRdr);

            var statusList =
                from status in statusXml.Elements("status")
                let dateParts = 
                    status.Element("created_at").Value.Split(' ')
                let createdAtDate = 
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT", 
                        dateParts[1], 
                        dateParts[2], 
                        dateParts[5], 
                        dateParts[3]))
                let user = status.Element("user")
                select
                   new Status
                   {
                       CreatedAt = createdAtDate,
                       Favorited = 
                        bool.Parse(
                            string.IsNullOrEmpty(status.Element("favorited").Value) ?
                            "true" :
                            status.Element("favorited").Value),
                       ID = status.Element("id").Value,
                       InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                       InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                       Source = status.Element("source").Value,
                       Text = status.Element("text").Value,
                       Truncated = bool.Parse(status.Element("truncated").Value),
                       User =
                           new User
                           {
                               Description = user.Element("description").Value,
                               FollowersCount = int.Parse(user.Element("followers_count").Value),
                               ID = user.Element("id").Value,
                               Location = user.Element("location").Value,
                               Name = user.Element("name").Value,
                               ProfileImageUrl = user.Element("profile_image_url").Value,
                               Protected = bool.Parse(user.Element("protected").Value),
                               ScreenName = user.Element("screen_name").Value,
                               URL = user.Element("url").Value
                           }
                   };

            return statusList.ToList();
        }
    }
}