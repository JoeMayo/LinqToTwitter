using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    ///Processes Twitter Saved Search requests.
    /// </summary>
    public class SavedSearchRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of search to perform (Searches or Show)
        /// </summary>
        public SavedSearchType Type { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        public ulong ID { get; set; }

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
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseEnum<SavedSearchType>(parameters["Type"]);

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
        Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
                throw new ArgumentException("ID is required for a Saved Search Show query.", "ID");

            ID = ulong.Parse(parameters["ID"]);

            var url = BaseUrl + "saved_searches/show/" + ID + ".json";

            return new Request(url);
        }

        /// <summary>
        /// return a saved searches url
        /// </summary>
        /// <returns>saved search url</returns>
        Request BuildSearchesUrl()
        {
            return new Request(BaseUrl + "saved_searches/list.json");
        }

        /// <summary>
        /// Transforms Twitter response into List of SavedSearch
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of SavedSearch</returns>
        public List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonElement searchJson = JsonDocument.Parse(responseJson).RootElement;

            List<SavedSearch> savedSearchList = new();

            switch (Type)
            {
                case SavedSearchType.Searches:
                    savedSearchList =
                        (from search in searchJson.EnumerateArray()
                         select new SavedSearch(search))
                        .ToList();
                    break;
                case SavedSearchType.Show:
                    savedSearchList = new List<SavedSearch> { new SavedSearch(searchJson) };
                    break;
                default:
                    break;
            }

            foreach (var search in savedSearchList)
            {
                search.Type = Type;
                search.ID = ID;
            };

            return savedSearchList.OfType<T>().ToList();
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement searchJson = JsonDocument.Parse(responseJson).RootElement;

            var search = new SavedSearch(searchJson);

            return search.ItemCast(default(T));
        }
    }
}
