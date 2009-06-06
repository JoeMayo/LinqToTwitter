using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Saved Search requests
    /// </summary>
    public class SavedSearchRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<SavedSearch>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                url = BuildSearchesUrl();
                return url;
            }

            SavedSearchType savedSearchType = RequestProcessorHelper.ParseQueryEnumType<SavedSearchType>(parameters["Type"]);

            switch (savedSearchType)
            {
                case SavedSearchType.Searches:
                    url = BuildSearchesUrl();
                    break;
                case SavedSearchType.Show:
                    url = BuildShowUrl(parameters);
                    break;
                default:
                    url = BuildSearchesUrl();
                    break;
            }

            return url;
        }

        /// <summary>
        /// construct a base show url
        /// </summary>
        /// <param name="url">base show url</param>
        /// <returns>base url + show segment</returns>
        private string BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
            {
                throw new ArgumentException("ID is required for a Saved Search Show query.", "ID");
            }

            var url = BaseUrl + "saved_searches/show.xml";

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            return url;
        }

        /// <summary>
        /// return a saved searches url
        /// </summary>
        /// <returns>saved search url</returns>
        private string BuildSearchesUrl()
        {
            return BaseUrl + "saved_searches.xml";
        }

        /// <summary>
        /// transforms XML into IList of SavedSearch
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IList of SavedSearch</returns>
        public IList ProcessResults(XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("saved_search").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "saved_search")
            {
                responseItems.Add(twitterResponse);
            }

            var tempPosition = 0;

            var savedSearchList =
                from search in responseItems
                let createdAtDate =
                    DateTime.ParseExact(
                        search.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        null)
                let position =
                    int.TryParse(search.Element("position").Value, out tempPosition)
                select
                   new SavedSearch
                   {
                       ID = search.Element("id").Value,
                       Name = search.Element("name").Value,
                       Query = search.Element("query").Value,
                       Postition = tempPosition,
                       CreatedAt = createdAtDate
                   };

            return savedSearchList.ToList();
        }
    }
}
