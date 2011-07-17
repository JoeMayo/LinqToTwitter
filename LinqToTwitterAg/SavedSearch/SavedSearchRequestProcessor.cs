using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Saved Search requests
    /// </summary>
    public class SavedSearchRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of search to perform (Searches or Show)
        /// </summary>
        private SavedSearchType Type { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        private string ID { get; set; }

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
        public Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<SavedSearchType>(parameters["Type"]);

            switch (Type)
            {
                case SavedSearchType.Searches:
                    return BuildSearchesUrl();
                case SavedSearchType.Show:
                    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// construct a base show url
        /// </summary>
        /// <param name="url">base show url</param>
        /// <returns>base url + show segment</returns>
        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
                throw new ArgumentException("ID is required for a Saved Search Show query.", "ID");

            ID = parameters["ID"];

            var url = BuildUrlHelper.TransformIDUrl(parameters, "saved_searches/show.xml");
            return new Request(BaseUrl + url);
        }

        /// <summary>
        /// return a saved searches url
        /// </summary>
        /// <returns>saved search url</returns>
        private Request BuildSearchesUrl()
        {
            return new Request(BaseUrl + "saved_searches.xml");
        }

        /// <summary>
        /// transforms XML into IList of SavedSearch
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of SavedSearch</returns>
        public List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<saved_searches></saved_searches>";
            }

            XElement twitterResponse = XElement.Parse(responseXml);

            List<XElement> responseItems = null;

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "saved_search")
            {
                responseItems.Add(twitterResponse);
            } 
            else if (twitterResponse.Elements("saved_search") == null)
            {
                responseItems = new List<XElement>();
            }
            else
            {
                responseItems = twitterResponse.Elements("saved_search").ToList();
            }

            var tempPosition = 0;

            var savedSearchList =
                from search in responseItems
                let createdAtDate =
                    DateTime.ParseExact(
                        search.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture)
                let position =
                    int.TryParse(search.Element("position").Value, out tempPosition)
                select
                   new SavedSearch
                   {
                       Type = Type,
                       ID = search.Element("id").Value,
                       Name = search.Element("name").Value,
                       Query = search.Element("query").Value,
                       Postition = tempPosition,
                       CreatedAt = createdAtDate
                   };

            return savedSearchList.OfType<T>().ToList();
        }
    }
}
