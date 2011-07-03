using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if SILVERLIGHT
#else
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Status requests
    /// </summary>
    public class RawRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Actual query string sent to twitter
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// TweetID
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return new ParameterFinder<Raw>(
               lambdaExpression.Body,
               new List<string> { 
                   "QueryString"
               })
               .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("QueryString"))
                throw new ArgumentNullException("QueryString", "QueryString parameter is required.");

            QueryString = parameters["QueryString"].Trim();

            if (QueryString == string.Empty)
                throw new ArgumentException("Blank QueryString isn't valid.", "QueryString");

            string url = BaseUrl.TrimEnd('/') + "/" + QueryString.TrimStart('/');
            var parts = url.Split('?');
            var req = new Request(parts[0]);
            var urlParams = req.RequestParameters;

            if (parts.Length == 2)
            {
                var qsParms =
                    (from pair in parts[1].Split('&')
                     let keyVal = pair.Split('=')
                     select new
                     {
                         Key = keyVal[0],
                         Val = keyVal[1]
                     })
                    .ToDictionary(
                        key => key.Key,
                        val => val.Val);

                //var qsparms = HttpUtility.ParseQueryString(url);  // probably will return an empty collection
                foreach (KeyValuePair<string, string> parm in qsParms)
                    urlParams.Add(new QueryParameter(parm.Key, parm.Value)); // may need to unescape
            }

            return req;
        }

        /// <summary>
        /// transforms XML into IQueryable of Status
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of Status</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            var rawList = new List<Raw>
            {
                new Raw
                {
                    QueryString = QueryString,
                    Result = responseXml
                }
            };

            return rawList.OfType<T>().ToList();
        }
    }
}
