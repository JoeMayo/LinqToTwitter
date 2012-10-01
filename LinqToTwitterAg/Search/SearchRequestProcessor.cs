using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using LinqToTwitter.Common;

using LitJson;

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#endif

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
        /// adds user information for each tweet if true (default = false)
        /// </summary>
        private bool ShowUser { get; set; }

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
        /// Return tweets since this date
        /// </summary>
        private DateTime Since { get; set; }

        /// <summary>
        /// for getting tweets with ID that is less than or equal to this value
        /// </summary>
        private ulong MaxID { get; set; }

        /// <summary>
        /// With exact phrase
        /// </summary>
        private string WordPhrase { get; set; }

        /// <summary>
        /// With all words
        /// </summary>
        private string WordAnd { get; set; }

        /// <summary>
        /// With any of the words
        /// </summary>
        private string WordOr { get; set; }

        /// <summary>
        /// Without the words
        /// </summary>
        private string WordNot { get; set; }

        /// <summary>
        /// With hashtag (add a single hashtag without the #)
        /// </summary>
        private string Hashtag { get; set; }

        /// <summary>
        /// From this person
        /// </summary>
        private string PersonFrom { get; set; }

        /// <summary>
        /// To this person
        /// </summary>
        private string PersonTo { get; set; }

        /// <summary>
        /// Person mentioned in tweet
        /// </summary>
        private string PersonReference { get; set; }

        /// <summary>
        /// Tweets with an attitude (Positive, Negative, or Question)
        /// </summary>
        private Attitude Attitude { get; set; }

        // TODO: can't find WithLinks or WithRetweets in docs - research to find out if they should be here
        /// <summary>
        /// Tweets that contain links
        /// </summary>
        private bool WithLinks { get; set; }

        /// <summary>
        /// Tweets that have been retweeted
        /// </summary>
        private bool WithRetweets { get; set; }

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
                       "SearchLanguage",
                       "Locale",
                       "Count",
                       "Page",
                       "Since",
                       "Until",
                       "SinceID",
                       "MaxID",
                       "GeoCode",
                       "ShowUser",
                       "ResultType",
                       "WordPhrase",
                       "WordAnd",
                       "WordOr",
                       "WordNot",
                       "Hashtag",
                       "PersonFrom",
                       "PersonTo",
                       "PersonReference",
                       "Attitude",
                       "WithLinks",
                       "WithRetweets",
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
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            // Joe: Why force a Type when there is only one Type?
            //
            // Answer: This is a LINQ to Twitter idiom and is necessary for extensibility.  
            // If we add another member to the enum in the future and this wasn't here, 
            // then adding this later would break a lot of code - Joe
            Type = RequestProcessorHelper.ParseQueryEnumType<SearchType>(parameters["Type"]);

            return BuildSearchUrlParameters(parameters, "tweets.json");
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

            if (parameters.ContainsKey("Query"))
            {
                Query = parameters["Query"];

                urlParams.Add(new QueryParameter("q", Query));
            }

            if (parameters.ContainsKey("ShowUser"))
            {
                ShowUser = bool.Parse(parameters["ShowUser"]);

                if (ShowUser)
                {
                    urlParams.Add(new QueryParameter("show_user", "true")); 
                }
            }

            if (parameters.ContainsKey("Since"))
            {
                Since = DateTime.Parse(parameters["Since"]).Date;
                urlParams.Add(new QueryParameter("since", Since.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
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

            if (parameters.ContainsKey("WordPhrase"))
            {
                WordPhrase = parameters["WordPhrase"];
                urlParams.Add(new QueryParameter("exact" , WordPhrase));
            }

            if (parameters.ContainsKey("WordAnd"))
            {
                WordAnd = parameters["WordAnd"];
                urlParams.Add(new QueryParameter("ands" , WordAnd));
            }

            if (parameters.ContainsKey("WordOr"))
            {
                WordOr = parameters["WordOr"];
                urlParams.Add(new QueryParameter("ors" , WordOr));
            }

            if (parameters.ContainsKey("WordNot"))
            {
                WordNot = parameters["WordNot"];
                urlParams.Add(new QueryParameter("nots" , WordNot));
            }

            if (parameters.ContainsKey("Hashtag"))
            {
                Hashtag = parameters["Hashtag"];
                urlParams.Add(new QueryParameter("tag" ,Hashtag));
            }

            if (parameters.ContainsKey("PersonFrom"))
            {
                PersonFrom = parameters["PersonFrom"];
                urlParams.Add(new QueryParameter("from", PersonFrom));
            }

            if (parameters.ContainsKey("PersonTo"))
            {
                PersonTo = parameters["PersonTo"];
                urlParams.Add(new QueryParameter("to" ,PersonTo));
            }

            if (parameters.ContainsKey("PersonReference"))
            {
                PersonReference = parameters["PersonReference"];
                urlParams.Add(new QueryParameter("ref" ,PersonReference));
            }

            if (parameters.ContainsKey("Attitude"))
            {
                Attitude = RequestProcessorHelper.ParseQueryEnumType<Attitude>(parameters["Attitude"]);

                if ((Attitude & Attitude.Positive) == Attitude.Positive)
                {
                    urlParams.Add(new QueryParameter("tude[]", ":)")); 
                }

                if ((Attitude & Attitude.Negative) == Attitude.Negative)
                {
                    urlParams.Add(new QueryParameter("tude[]", ":("));
                }

                if ((Attitude & Attitude.Question) == Attitude.Question)
                {
                    urlParams.Add(new QueryParameter("tude[]", "?"));
                }
            }

            if (parameters.ContainsKey("WithLinks"))
            {
                WithLinks = bool.Parse(parameters["WithLinks"]);

                if (WithLinks)
                {
                    urlParams.Add(new QueryParameter("filter[]", "links"));
                }
            }

            if (parameters.ContainsKey("WithRetweets"))
            {
                WithRetweets = bool.Parse(parameters["WithRetweets"]);

                if (WithRetweets)
                {
                    urlParams.Add(new QueryParameter("include[]", "retweets"));
                }
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

            if (string.IsNullOrEmpty(responseJson))
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
                ShowUser = ShowUser,
                SinceID = SinceID,
                SearchLanguage = SearchLanguage,
                Locale = Locale,
                Since = Since.Date,
                Until = Until.Date,
                ResultType = ResultType,
                WordPhrase = WordPhrase,
                WordAnd = WordAnd,
                WordOr = WordOr,
                WordNot = WordNot,
                Hashtag = Hashtag,
                PersonFrom = PersonFrom,
                PersonTo = PersonTo,
                PersonReference = PersonReference,
                Attitude = Attitude,
                //WithLinks = WithLinks,
                WithRetweets = WithRetweets,
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