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
        internal IQueryable Execute(Expression expression)
        {
            Dictionary<string, string> parameters = null;

            var reqProc = CreateRequestProcessor(expression);

            var whereFinder = new FirstWhereClauseFinder();
            var whereExpression = whereFinder.GetFirstWhere(expression);

            if (whereExpression != null)
            {
                var lambdaExpression =
                    (LambdaExpression)
                    ((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                lambdaExpression =
                    (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                parameters = reqProc.GetParameters(lambdaExpression);
            }

            var url = reqProc.BuildURL(parameters);
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
                case "Status":
                    req = new StatusRequestProcessor() { BaseUrl = BaseUrl };
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
            var req = HttpWebRequest.Create(url);
            req.Credentials = new NetworkCredential(UserName, Password);
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

        /// <summary>
        /// performs HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        private IQueryable ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor)
        {
            var req = WebRequest.Create(url);
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
        /// sends a status update
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

        /// <summary>
        /// deletes a tweet
        /// </summary>
        /// <param name="id">id of tweet</param>
        /// <returns>deleted tweet</returns>
        public IQueryable<Status> Destroy(string id)
        {
            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".xml";

            var results =
                ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    new StatusRequestProcessor());

            return results as IQueryable<Status>;
        }
    }
}