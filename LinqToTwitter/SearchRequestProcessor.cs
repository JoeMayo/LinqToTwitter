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
    class SearchRequestProcessor : IRequestProcessor
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
        /// last status ID
        /// </summary>
        private ulong SinceID { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        private string GeoCode { get; set; }

        /// <summary>
        /// adds user information for each tweet if true (default = false)
        /// </summary>
        private string ShowUser { get; set; }

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
                       "SinceID",
                       "GeoCode",
                       "ShowUser"
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
                ShowUser = parameters["ShowUser"];
                urlParams.Add("show_user=" + parameters["ShowUser"]);
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

//        private string m_testQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
//<feed xmlns:google=""http://base.google.com/ns/1.0"" xml:lang=""en-US"" xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:twitter=""http://api.twitter.com/"">
//  <id>tag:search.twitter.com,2005:search/LINQ to Twitter</id>
//  <link type=""text/html"" rel=""alternate"" href=""http://search.twitter.com/search?q=LINQ+to+Twitter""/>
//  <link type=""application/atom+xml"" rel=""self"" href=""http://search.twitter.com/search.atom?lang=en&amp;q=LINQ%20to%20Twitter&amp;rpp=2&amp;show_user=true""/>
//  <title>LINQ to Twitter - Twitter Search</title>
//  <link type=""application/opensearchdescription+xml"" rel=""search"" href=""http://search.twitter.com/opensearch.xml""/>
//  <link type=""application/atom+xml"" rel=""refresh"" href=""http://search.twitter.com/search.atom?lang=en&amp;q=LINQ+to+Twitter&amp;rpp=2&amp;show_user=true&amp;since_id=1600414821""/>
//  <twitter:warning>adjusted since_id, it was older than allowed</twitter:warning>
//  <updated>2009-04-23T08:33:40Z</updated>
//  <openSearch:itemsPerPage>2</openSearch:itemsPerPage>
//  <openSearch:language>en</openSearch:language>
//  <link type=""application/atom+xml"" rel=""next"" href=""http://search.twitter.com/search.atom?lang=en&amp;max_id=1600414821&amp;page=2&amp;q=LINQ+to+Twitter&amp;rpp=2""/>
//  <entry>
//    <id>tag:search.twitter.com,2005:1592662820</id>
//    <published>2009-04-23T08:33:40Z</published>
//    <link type=""text/html"" rel=""alternate"" href=""http://twitter.com/slecluyse/statuses/1592662820""/>
//    <title>slecluyse: LINQ to Twitter http://bit.ly/11vAp</title>
//    <content type=""html"">slecluyse: &lt;b&gt;LINQ&lt;/b&gt; &lt;b&gt;to&lt;/b&gt; &lt;b&gt;Twitter&lt;/b&gt; &lt;a href=""http://bit.ly/11vAp""&gt;http://bit.ly/11vAp&lt;/a&gt;</content>
//    <updated>2009-04-23T08:33:40Z</updated>
//    <link type=""image/png"" rel=""image"" href=""http://s3.amazonaws.com/twitter_production/profile_images/178528955/linkedin_normal.jpg""/>
//    <twitter:source>&lt;a href=""http://www.tweetdeck.com/""&gt;TweetDeck&lt;/a&gt;</twitter:source>
//    <twitter:lang>en</twitter:lang>
//    <author>
//      <name>slecluyse (Steven Lecluyse)</name>
//      <uri>http://twitter.com/slecluyse</uri>
//    </author>
//  </entry>
//  <entry>
//    <id>tag:search.twitter.com,2005:1575147318</id>
//    <published>2009-04-21T14:12:41Z</published>
//    <link type=""text/html"" rel=""alternate"" href=""http://twitter.com/JackStow/statuses/1575147318""/>
//    <title>JackStow: Blog post: A Simple Twitter Status ASP.NET User Control With LINQ To XML: 
//The blog system I'm us.. http://tinyurl.com/cvdbvr</title>
//    <content type=""html"">JackStow: Blog post: A Simple &lt;b&gt;Twitter&lt;/b&gt; Status ASP.NET User Control With &lt;b&gt;LINQ&lt;/b&gt; &lt;b&gt;To&lt;/b&gt; XML: 
//The blog system I'm us.. &lt;a href=""http://tinyurl.com/cvdbvr""&gt;http://tinyurl.com/cvdbvr&lt;/a&gt;</content>
//    <updated>2009-04-21T14:12:41Z</updated>
//    <link type=""image/png"" rel=""image"" href=""http://s3.amazonaws.com/twitter_production/profile_images/63057949/Twitter-Jack-Small_normal.jpg""/>
//    <twitter:source>&lt;a href=""http://twitterfeed.com""&gt;twitterfeed&lt;/a&gt;</twitter:source>
//    <twitter:lang>en</twitter:lang>
//    <author>
//      <name>JackStow (Jack Stow)</name>
//      <uri>http://twitter.com/JackStow</uri>
//    </author>
//  </entry>
//</feed>";

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public IList ProcessResults(XElement twitterResponse)
        {
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
                            atomEntry.Element(twitter + "geo").Value
                     }).ToList()
            };

            var searchList = new List<Search>
            {
                searchResult
            };

            return searchList;
        }
    }
}