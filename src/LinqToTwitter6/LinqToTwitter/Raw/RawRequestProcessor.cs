using LinqToTwitter.Provider;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    /// <summary>
    /// Raw requests, allowing free-form url and query strings.
    /// </summary>
    public class RawRequestProcessor<T> : 
        IRequestProcessor<T>,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// Actual query string sent to twitter
        /// </summary>
        internal string? QueryString { get; set; }

        /// <summary>
        /// TweetID
        /// </summary>
        internal string? Result { get; set; }

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
                   nameof(QueryString)
               })
               .Parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const int UrlPart = 0;
            const int UrlPlusParameters = 2;

            if (parameters == null || !parameters.ContainsKey(nameof(QueryString)))
                throw new ArgumentNullException(nameof(QueryString), $"{nameof(QueryString)} parameter is required.");

            QueryString = parameters[nameof(QueryString)].Trim();

            if (QueryString == string.Empty)
                throw new ArgumentException($"Blank {nameof(QueryString)} isn't valid.", nameof(QueryString));

            string url = BaseUrl?.TrimEnd('/') + "/" + QueryString.TrimStart('/');
            var parts = url.Split('?');
            var req = new Request(parts[UrlPart]);
            var urlParams = req.RequestParameters;

            if (parts.Length == UrlPlusParameters)
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

                foreach (KeyValuePair<string, string> parm in qsParms)
                    urlParams.Add(new QueryParameter(parm.Key, parm.Value));
            }

            return req;
        }

        /// <summary>
        /// transforms response into List of Raw
        /// </summary>
        /// <param name="response">Twitter response</param>
        /// <returns>List of Raw</returns>
        public virtual List<T> ProcessResults(string response)
        {
            var rawList = new List<Raw>
            {
                new Raw
                {
                    QueryString = QueryString,
                    Response = response
                }
            };

            return rawList.OfType<T>().ToList();
        }

        [return: MaybeNull]
        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            var raw = new Raw
            {
                Response = responseJson
            };

            return raw!.ItemCast(default(T));
        }
    }
}
