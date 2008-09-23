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
using System.Diagnostics;

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

            //var statusList = ExecuteTwitterQuery<Status>(parameters);

            var req = CreateRequestProcessor<Status>();

            var url = req.BuildURL(parameters);
            var queryableList = QueryTwitter(url, req);
            //var queryableList = res.AsQueryable<Status>();

            return queryableList;
        }

        ///// <summary>
        ///// constructs the Twitter API URL and executes the query
        ///// </summary>
        ///// <param name="parameters">criteria for the query</param>
        ///// <returns>List of Status objects</returns>
        //private object ExecuteTwitterQuery<T>(Dictionary<string, string> parameters)
        //{
        //    var req = CreateRequestProcessor<T>();
        //    var url = req.BuildURL(parameters);
        //    var res = QueryTwitter<T>(url);

        //    return res;
        //}

        /// <summary>
        /// factory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        public IRequestProcessor CreateRequestProcessor<T>()
        {
            IRequestProcessor req;

            switch (typeof(T).Name)
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

        ///// <summary>
        ///// constructs an URL, based on type and parameter
        ///// </summary>
        ///// <typeparam name="T">type of query</typeparam>
        ///// <param name="parameters">parameters for building URL</param>
        ///// <returns>final URL to call Twitter API with</returns>
        //private string BuildUrl<T>(Dictionary<string, string> parameters)
        //{
        //    string url = null;

        //    if (parameters == null ||
        //        !parameters.ContainsKey("Type"))
        //    {
        //        url = BaseUrl + "statuses/public_timeline.xml";
        //        return url;
        //    }

        //    switch (typeof(T).Name)
        //    {
        //        case "Status":
        //            switch (parameters["Type"])
        //            {
        //                case "Public":
        //                    url = BaseUrl + "statuses/public_timeline.xml";
        //                    break;
        //                case "Friends":
        //                    url = BaseUrl + "statuses/friends_timeline.xml";
        //                    break;
        //                default:
        //                    url = BaseUrl + "statuses/public_timeline.xml";
        //                    break;
        //            }
        //            break;
        //        default:
        //            url = BaseUrl + "statuses/public_timeline.xml";
        //            break;
        //    }

        //    return url;
        //}

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        private object QueryTwitter(string url, IRequestProcessor requestProcessor)
        {
            var req = HttpWebRequest.Create(url);
            req.Credentials = new NetworkCredential(UserName, Password);
            var resp = req.GetResponse();
            var strm = resp.GetResponseStream();
            var strmRdr = new StreamReader(strm);
            var txtRdr = new StringReader(strmRdr.ReadToEnd());
            var statusXml = XElement.Load(txtRdr);

            var results = requestProcessor.ProcessResults(statusXml);
            return results;
        }
    }
}