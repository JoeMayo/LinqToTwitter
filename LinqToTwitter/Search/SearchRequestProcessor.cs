using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Collections;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// processes search queries
    /// </summary>
    class SearchRequestProcessor<T> : IRequestProcessor<T>
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
        /// filters query to tweets in specified language (ISO 639-1)
        /// </summary>
        private string SearchLanguage { get; set; }

        /// <summary>
        /// language of the search query (currently only supports ja)
        /// </summary>
        private string Locale { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        private int PageSize { get; set; }

        /// <summary>
        /// page number
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// Return tweets since this date
        /// </summary>
        private DateTime Since { get; set; }

        /// <summary>
        /// Return tweets before this date
        /// </summary>
        private DateTime Until { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        private ulong SinceID { get; set; }

        /// <summary>
        /// for getting tweets with ID that is less than or equal to this value
        /// </summary>
        private ulong MaxID { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        private string GeoCode { get; set; }

        /// <summary>
        /// adds user information for each tweet if true (default = false)
        /// </summary>
        private bool ShowUser { get; set; }

        /// <summary>
        /// Metadata for type of result (mixed, recent, or popular)
        /// </summary>
        private ResultType ResultType { get; set; }

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
                       "WithRetweets"
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<SearchType>(parameters["Type"]);

            var url = BaseUrl + "search.atom";

            url = BuildSearchUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildSearchUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("GeoCode"))
            {
                GeoCode = parameters["GeoCode"];
                urlParams.Add("geocode=" + HttpUtility.UrlEncode(parameters["GeoCode"]));
            }

            if (parameters.ContainsKey("SearchLanguage"))
            {
                SearchLanguage = parameters["SearchLanguage"];
                urlParams.Add("lang=" + parameters["SearchLanguage"]);
            }

            if (parameters.ContainsKey("Locale"))
            {
                Locale = parameters["Locale"];
                urlParams.Add("locale=" + parameters["Locale"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("PageSize"))
            {
                PageSize = int.Parse(parameters["PageSize"]);
                urlParams.Add("rpp=" + parameters["PageSize"]);
            }

            if (parameters.ContainsKey("Query"))
            {
                Query = parameters["Query"];

                if (Query.Length > 140)
                {
                    throw new ArgumentException("Query length must be 140 characters or less.", "Query");
                }

                urlParams.Add("q=" + HttpUtility.UrlEncode(parameters["Query"]));
            }

            if (parameters.ContainsKey("ShowUser"))
            {
                ShowUser = bool.Parse(parameters["ShowUser"]);

                if (ShowUser)
                {
                    urlParams.Add("show_user=" + parameters["ShowUser"]); 
                }
            }

            if (parameters.ContainsKey("Since"))
            {
                Since = DateTime.Parse(parameters["Since"]);
                urlParams.Add("since=" + Since.ToString("yyyy-MM-dd"));
            }

            if (parameters.ContainsKey("Until"))
            {
                Until = DateTime.Parse(parameters["Until"]);
                urlParams.Add("until=" + Until.ToString("yyyy-MM-dd"));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add("max_id=" + parameters["MaxID"]);
            }

            if (parameters.ContainsKey("ResultType"))
            {
                ResultType = RequestProcessorHelper.ParseQueryEnumType<ResultType>(parameters["ResultType"]);
                urlParams.Add("result_type=" + ResultType.ToString().ToLower());
            }

            if (parameters.ContainsKey("WordPhrase"))
            {
                WordPhrase = parameters["WordPhrase"];
                urlParams.Add("exact=" + HttpUtility.UrlEncode(parameters["WordPhrase"]));
            }

            if (parameters.ContainsKey("WordAnd"))
            {
                WordAnd = parameters["WordAnd"];
                urlParams.Add("ands=" + HttpUtility.UrlEncode(parameters["WordAnd"]));
            }

            if (parameters.ContainsKey("WordOr"))
            {
                WordOr = parameters["WordOr"];
                urlParams.Add("ors=" + HttpUtility.UrlEncode(parameters["WordOr"]));
            }

            if (parameters.ContainsKey("WordNot"))
            {
                WordNot = parameters["WordNot"];
                urlParams.Add("nots=" + HttpUtility.UrlEncode(parameters["WordNot"]));
            }

            if (parameters.ContainsKey("Hashtag"))
            {
                Hashtag = parameters["Hashtag"];
                urlParams.Add("tag=" + HttpUtility.UrlEncode(parameters["Hashtag"]));
            }

            if (parameters.ContainsKey("PersonFrom"))
            {
                PersonFrom = parameters["PersonFrom"];
                urlParams.Add("from=" + HttpUtility.UrlEncode(parameters["PersonFrom"]));
            }

            if (parameters.ContainsKey("PersonTo"))
            {
                PersonTo = parameters["PersonTo"];
                urlParams.Add("to=" + HttpUtility.UrlEncode(parameters["PersonTo"]));
            }

            if (parameters.ContainsKey("PersonReference"))
            {
                PersonReference = parameters["PersonReference"];
                urlParams.Add("ref=" + HttpUtility.UrlEncode(parameters["PersonReference"]));
            }

            if (parameters.ContainsKey("Attitude"))
            {
                Attitude = RequestProcessorHelper.ParseQueryEnumType<Attitude>(parameters["Attitude"]);

                if ((Attitude & Attitude.Positive) == Attitude.Positive)
                {
                    urlParams.Add("tude%5B%5D=%3A%29"); 
                }

                if ((Attitude & Attitude.Negative) == Attitude.Negative)
                {
                    urlParams.Add("tude%5B%5D=%3A%28");
                }

                if ((Attitude & Attitude.Question) == Attitude.Question)
                {
                    urlParams.Add("tude%5B%5D=%3F");
                }
            }

            if (parameters.ContainsKey("WithLinks"))
            {
                WithLinks = bool.Parse(parameters["WithLinks"]);

                if (WithLinks)
                {
                    urlParams.Add("filter%5B%5D=links");
                }
            }

            if (parameters.ContainsKey("WithRetweets"))
            {
                WithRetweets = bool.Parse(parameters["WithRetweets"]);

                if (WithRetweets)
                {
                    urlParams.Add("include%5B%5D=retweets");
                }
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
        public List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace twitter = "http://api.twitter.com/";
            XNamespace openSearch = "http://a9.com/-/spec/opensearch/1.1/";

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
                MaxID = MaxID,
                Since = Since,
                Until = Until,
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
                ID = twitterResponse.Element(atom + "id").Value,
                Title = twitterResponse.Element(atom + "title").Value,
                TwitterWarning = 
                    twitterResponse.Element(twitter + "warning") == null ?
                    string.Empty :
                    twitterResponse.Element(twitter + "warning").Value,
                Updated = DateTime.Parse(twitterResponse.Element(atom + "updated").Value, CultureInfo.InvariantCulture),
                ItemsPerPage = 
                    twitterResponse.Element(openSearch + "itemsPerPage") == null ?
                    -1 :
                    int.Parse(twitterResponse.Element(openSearch + "itemsPerPage").Value),
                Language = 
                    twitterResponse.Element(openSearch + "language") == null ?
                    string.Empty :
                    twitterResponse.Element(openSearch + "language").Value,
                Alternate = 
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "alternate").Count() == 0 ?
                    string.Empty :
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "alternate")
                        .First()
                        .Attribute("href").Value,
                Self = 
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "self").Count() == 0 ?
                    string.Empty :
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "self")
                        .First()
                        .Attribute("href").Value,
                Search = 
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "search").Count() == 0 ?
                    string.Empty :
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "search")
                        .First()
                        .Attribute("href").Value,
                Refresh = 
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "refresh").Count() == 0 ?
                    string.Empty :
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "refresh")
                        .First()
                        .Attribute("href").Value,
                Next =
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "next").Count() == 0 ?
                    string.Empty :
                    twitterResponse.Elements(atom + "link")
                        .Where(elem => elem.Attribute("rel").Value == "next")
                        .First()
                        .Attribute("href").Value,
                Entries =
                    (from node in twitterResponse.Nodes()
                     let atomEntry = node as XElement
                     where atomEntry != null && atomEntry.Name == atom + "entry"
                     let author = atomEntry.Element(atom + "author")
                     select new AtomEntry
                     {
                         ID = atomEntry.Element(atom + "id").Value,
                         Published = DateTime.Parse(atomEntry.Element(atom + "published").Value, CultureInfo.InvariantCulture),
                         Title = atomEntry.Element(atom + "title").Value,
                         Content = atomEntry.Element(atom + "content").Value,
                         Updated = DateTime.Parse(atomEntry.Element(atom + "updated").Value, CultureInfo.InvariantCulture),
                         Source = atomEntry.Element(twitter + "source").Value,
                         Language = atomEntry.Element(twitter + "lang").Value,
                         Alternate = atomEntry.Elements(atom + "link")
                                     .Where(elem => elem.Attribute("rel").Value == "alternate")
                                     .First()
                                     .Attribute("href").Value,
                         Image = atomEntry.Elements(atom + "link")
                                     .Where(elem => elem.Attribute("rel").Value == "image")
                                     .First()
                                     .Attribute("href").Value,
                         Author = new AtomAuthor
                         {
                             Name = author.Element(atom + "name").Value,
                             URI = author.Element(atom + "uri").Value
                         },
                         Location = 
                            atomEntry.Element(twitter + "geo") == null ?
                            string.Empty :
                            atomEntry.Element(twitter + "geo").Value,
                         ResultType =
                            atomEntry.Element(twitter + "metadata") == null ||
                            atomEntry.Element(twitter + "metadata")
                                .Element(twitter + "result_type") == null ?
                            string.Empty :
                            atomEntry
                                .Element(twitter + "metadata")
                                    .Element(twitter + "result_type").Value
                     }).ToList()
            };

            var searchList = new List<Search>
            {
                searchResult
            };

            return searchList.OfType<T>().ToList();
        }
    }
}