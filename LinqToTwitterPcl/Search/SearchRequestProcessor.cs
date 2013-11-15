using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes search queries
    /// </summary>
    class SearchRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        private SearchType Type { get; set; }

        /// <summary>
        /// search query
        /// </summary>
        private string Query { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        private string GeoCode { get; set; }

        /// <summary>
        /// filters query to tweets in specified language (ISO 639-1)
        /// </summary>
        private string SearchLanguage { get; set; }

        /// <summary>
        /// language of the search query (currently only supports ja)
        /// </summary>
        private string Locale { get; set; }

        /// <summary>
        /// Metadata for type of result (mixed, recent, or popular)
        /// </summary>
        private ResultType ResultType { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        /// Return tweets before this date
        /// </summary>
        private DateTime Until { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        private ulong SinceID { get; set; }

        /// <summary>
        /// Include entities in results (default: false)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// for getting tweets with ID that is less than or equal to this value
        /// </summary>
        private ulong MaxID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Search>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Query",
                       "GeoCode",
                       "SearchLanguage",
                       "Locale",
                       "ResultType",
                       "Count",
                       "Until",
                       "SinceID",
                       "MaxID",
                       "IncludeEntities"
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("Type"))
                Type = RequestProcessorHelper.ParseQueryEnumType<SearchType>(parameters["Type"]);

            return BuildSearchUrlParameters(parameters, "search/tweets.json");
        }

        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildSearchUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Query") && !string.IsNullOrWhiteSpace(parameters["Query"]))
            {
                Query = parameters["Query"];

                urlParams.Add(new QueryParameter("q", Query));
            }
            else
            {
                throw new ArgumentNullException("Query", "Query filter in where clause is required.");
            }

            if (parameters.ContainsKey("GeoCode"))
            {
                GeoCode = parameters["GeoCode"];
                urlParams.Add(new QueryParameter("geocode" , GeoCode));
            }

            if (parameters.ContainsKey("SearchLanguage"))
            {
                SearchLanguage = parameters["SearchLanguage"];
                urlParams.Add(new QueryParameter("lang", SearchLanguage));
            }

            if (parameters.ContainsKey("Locale"))
            {
                Locale = parameters["Locale"];
                urlParams.Add(new QueryParameter("locale", Locale));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", Count.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Until"))
            {
                Until = DateTime.Parse(parameters["Until"]).Date;
                urlParams.Add(new QueryParameter("until",  Until.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", SinceID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", MaxID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("ResultType"))
            {
                ResultType = RequestProcessorHelper.ParseQueryEnumType<ResultType>(parameters["ResultType"]);
                urlParams.Add(new QueryParameter("result_type" , ResultType.ToString().ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Search
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Search</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<Search> search;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                search = new List<Search> { new Search() };
            }
            else
            {
                var searchResult = JsonSerialize(responseJson);

                search = new List<Search> { searchResult };
            }

            return search.OfType<T>().ToList();
        }

        Search JsonSerialize(string responseJson)
        {
            JsonData search = JsonMapper.ToObject(responseJson);

            var searchResult = new Search
            {
                Type = Type,
                GeoCode = GeoCode,
                Count = Count,
                Query = Query,
                MaxID = MaxID,
                SinceID = SinceID,
                SearchLanguage = SearchLanguage,
                Locale = Locale,
                Until = Until.Date,
                ResultType = ResultType,
                IncludeEntities = IncludeEntities,
                Statuses =
                    (from JsonData result in search["statuses"]
                     select new Status(result))
                    .ToList(),
                SearchMetaData = 
                    new SearchMetaData(search.GetValue<JsonData>("search_metadata"))
            };

            return searchResult;
        }
    }
}