using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// helps process related results requests
    /// </summary>
    public class RelatedResultsRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of related result to query i.e. Show
        /// </summary>
        private RelatedResultsType Type { get; set; }

        /// <summary>
        /// Tweet ID to get results for
        /// </summary>
        private ulong StatusID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<RelatedResults>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "StatusID"
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
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<RelatedResultsType>(parameters["Type"]);

            switch (Type)
            {
                case RelatedResultsType.Show:
                    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds an URL for finding results related to a specific tweet
        /// </summary>
        /// <param name="parameters">Parameters contain StatusID.</param>
        /// <returns>Url for performing related results show query.</returns>
        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("StatusID"))
                throw new ArgumentException("StatusID is a required parameter.", "StatusWeoID");

            StatusID = ulong.Parse(parameters["StatusID"]);
            var url = BuildUrlHelper.TransformParameterUrl(parameters, "StatusID", "related_results/show.json");
                        
            var req = new Request(BaseUrl + url);
            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Trend
        /// </summary>
        /// <param name="responseXml">XML response from Twitter</param>
        /// <returns>List of Trend</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);

            List<RelatedResults> results = null;

            if (twitterResponse.Element("item") != null &&
                twitterResponse.Element("item").Element("results") != null &&
                twitterResponse.Element("item").Element("results").Elements("item") != null)
            {
                results =
                        (from result in twitterResponse.Element("item").Element("results").Elements("item")
                         select RelatedResults.CreateRelatedResults(result))
                        .ToList();

                results.ForEach(result =>
                    {
                        result.StatusID = StatusID;
                        result.Type = Type;
                    }); 
            }
            else
            {
                results = new List<RelatedResults>();
            }

            return results.OfType<T>().ToList();
        }
    }
}
