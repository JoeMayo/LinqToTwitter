using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        /// page number
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// Metadata for type of result (mixed, recent, or popular)
        /// </summary>
        private ResultType ResultType { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        private int PageSize { get; set; }

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
        /// Include entities in results
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
                       "PageSize",
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
        public Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            // Joe: Why force a Type when there is only one Type?
            //
            // Answer: This is a LINQ to Twitter idiom and is necessary for extensibility.  
            // If we add another member to the enum in the future and this wasn't here, 
            // then adding this later would break a lot of code - Joe
            Type = RequestProcessorHelper.ParseQueryEnumType<SearchType>(parameters["Type"]);

            return BuildSearchUrlParameters(parameters, "search.json");
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

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", Page.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("PageSize"))
            {
                PageSize = int.Parse(parameters["PageSize"]);
                urlParams.Add(new QueryParameter("rpp", PageSize.ToString(CultureInfo.InvariantCulture)));
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

                if (IncludeEntities)
                {
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
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
            IEnumerable<Search> search = Enumerable.Empty<Search>();
            var serializer = Json.SearchConverter.GetSerializer();
            var parsedResponse = serializer.Deserialize<Json.Search>(responseJson);

            if (!string.IsNullOrEmpty(responseJson))
            {
                var searchResult = new Search
                {
                    Type = Type,
                    GeoCode = GeoCode,
                    Page = Page,
                    PageSize = PageSize,
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
                    WithLinks = WithLinks,
                    WithRetweets = WithRetweets,
                    IncludeEntities = IncludeEntities,
                    CompletedIn = parsedResponse.completed_in,
                    MaxID = parsedResponse.max_id,
                    NextPage = parsedResponse.next_page,
                    PageResult = parsedResponse.page,
                    QueryResult = parsedResponse.query,
                    ResultsPerPageResult = parsedResponse.results_per_page,
                    SinceIDResult = parsedResponse.since_id,
                    RefreshUrl = parsedResponse.refresh_url,
                    Results =
                        (from result in parsedResponse.results
                         select new SearchEntry
                         {
                             CreatedAt = DateTimeOffset.Parse(result.created_at),
                             Entities = new Entities
                             {
                                 HashTagMentions =
                                    result.entities == null || result.entities.hashes == null ?
                                        new List<HashTagMention>() :
                                        (from hash in result.entities.hashes
                                         select new HashTagMention
                                         {
                                             Tag = hash.text,
                                             Start = hash.start,
                                             End = hash.stop
                                         })
                                        .ToList(),
                                 MediaMentions =
                                    result.entities == null || result.entities.media == null ?
                                        new List<MediaMention>() :
                                        (from media in result.entities.media
                                         select new MediaMention
                                         {
                                             DisplayUrl = media.display_url,
                                             ExpandedUrl = media.expanded_url,
                                             ID = media.id,
                                             MediaUrl = media.media_url,
                                             MediaUrlHttps = media.media_url_https,
                                             Sizes =
                                                (from size in media.sizes
                                                 select new PhotoSize
                                                 {
                                                     Type = size.type,
                                                     Width = size.w,
                                                     Height = size.h,
                                                     Resize = size.resize
                                                 })
                                                .ToList(),
                                             Type = media.type,
                                             Url = media.url,
                                             Start = media.start,
                                             End = media.stop
                                         })
                                        .ToList(),
                                 UrlMentions =
                                    result.entities == null || result.entities.urls == null ?
                                        new List<UrlMention>() :
                                        (from url in result.entities.urls
                                         select new UrlMention
                                         {
                                             Url = url.url,
                                             DisplayUrl = url.display_url,
                                             ExpandedUrl = url.expanded_url,
                                             Start = url.start,
                                             End = url.stop
                                         })
                                        .ToList(),
                                 UserMentions =
                                    result.entities == null || result.entities.users == null ?
                                        new List<UserMention>() :
                                        (from user in result.entities.users
                                         select new UserMention
                                         {
                                             ScreenName = user.screen_name,
                                             Name = user.name,
                                             Id = user.id,
                                             Start = user.start,
                                             End = user.stop
                                         })
                                        .ToList()
                             },
                             FromUser = result.from_user,
                             FromUserID = result.from_user_id,
                             FromUserName = result.from_user_name,
                             Geo =
                                result.geo == null ?
                                    new Geometry { Coordinates = new List<Coordinate>() } :
                                    new Geometry
                                    {
                                        Type = result.geo.type,
                                        Coordinates = new List<Coordinate>
                                        {
                                            new Coordinate
                                            {
                                                Latitude = result.geo.latitude,
                                                Longitude = result.geo.longitude
                                            }
                                        }
                                    },
                             ID = result.id,
                             IsoLanguageCode = result.iso_language_code,
                             MetaData = new SearchMetaData
                             {
                                 RecentRetweets = result.metadata.recent_retweets,
                                 ResultType = (ResultType)Enum.Parse(typeof(ResultType), result.metadata.result_type, true)
                             },
                             ProfileImageUrl = result.profile_image_url,
                             ProfileImageUrlHttps = result.profile_image_url_https,
                             Source = result.source,
                             Text = result.text,
                             ToUser = result.to_user,
                             ToUserID = result.to_user_id,
                             ToUserName = result.to_user_name
                         }).ToList()
                };

                search = new List<Search> { searchResult };
            }

            return search.OfType<T>().ToList();
        }
    }
}