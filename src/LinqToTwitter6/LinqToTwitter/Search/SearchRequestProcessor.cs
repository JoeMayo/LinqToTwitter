using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes search queries
    /// </summary>
    public class SearchRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        internal SearchType Type { get; set; }

        /// <summary>
        /// search query
        /// </summary>
        internal string? Query { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        internal string? GeoCode { get; set; }

        /// <summary>
        /// filters query to tweets in specified language (ISO 639-1)
        /// </summary>
        internal string? SearchLanguage { get; set; }

        /// <summary>
        /// language of the search query (currently only supports ja)
        /// </summary>
        internal string? Locale { get; set; }

        /// <summary>
        /// Metadata for type of result (mixed, recent, or popular)
        /// </summary>
        private ResultType ResultType { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// Return tweets before this date
        /// </summary>
        internal DateTime Until { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        internal ulong SinceID { get; set; }

        /// <summary>
        /// Include entities in results (default: false)
        /// </summary>
        internal bool IncludeEntities { get; set; }

        /// <summary>
        /// for getting tweets with ID that is less than or equal to this value
        /// </summary>
        internal ulong MaxID { get; set; }

        /// <summary>
        /// Supports compatibility or extended mode tweets.
        /// </summary>
        internal TweetMode TweetMode { get; set; }

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
                       nameof(Type),
                       nameof(Query),
                       nameof(GeoCode),
                       nameof(SearchLanguage),
                       nameof(Locale),
                       nameof(ResultType),
                       nameof(Count),
                       nameof(Until),
                       nameof(SinceID),
                       nameof(MaxID),
                       nameof(IncludeEntities),
                       nameof(TweetMode)
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
                Type = RequestProcessorHelper.ParseEnum<SearchType>(parameters["Type"]);
            else
                throw new ArgumentException("Type is required", "Type");

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
                ResultType = RequestProcessorHelper.ParseEnum<ResultType>(parameters["ResultType"]);
                urlParams.Add(new QueryParameter("result_type" , ResultType.ToString().ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey(nameof(TweetMode)))
            {
                TweetMode = RequestProcessorHelper.ParseEnum<TweetMode>(parameters[nameof(TweetMode)]);
                urlParams.Add(new QueryParameter("tweet_mode", TweetMode.ToString().ToLower()));
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
            JsonElement search = JsonDocument.Parse(responseJson).RootElement;

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
                    (from result in search.GetProperty("statuses").EnumerateArray()
                     select new Status(result))
                    .ToList(),
                SearchMetaData = 
                    new SearchMetaData(search.GetProperty("search_metadata")),
                TweetMode = TweetMode
            };

            return searchResult;
        }
    }
}