using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// for performing Twitter searches
    /// </summary>
    public class Search : AtomFeed//, ITwitterEntity
    {
        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        public SearchType Type { get; set; }

        /// <summary>
        /// search query
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// filters query to tweets in specified language (ISO 639-1)
        /// </summary>
        public string SearchLanguage { get; set; }

        /// <summary>
        /// language of the search query (currently only supports ja)
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Return tweets since this date
        /// </summary>
        public DateTime Since { get; set; }

        /// <summary>
        /// Return tweets before this date
        /// </summary>
        public DateTime Until { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// for getting tweets with ID that is less than or equal to this value
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        public string GeoCode { get; set; }

        /// <summary>
        /// adds user information for each tweet if true (default = false)
        /// </summary>
        public bool ShowUser { get; set; }

        /// <summary>
        /// Metadata for type of result requested (mixed, recent, or popular)
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        /// With exact phrase
        /// </summary>
        public string WordPhrase { get; set; }

        /// <summary>
        /// With all words
        /// </summary>
        public string WordAnd { get; set; }

        /// <summary>
        /// With any of the words
        /// </summary>
        public string WordOr { get; set; }

        /// <summary>
        /// Without the words
        /// </summary>
        public string WordNot { get; set; }

        /// <summary>
        /// With hashtag (add a single hashtag without the #)
        /// </summary>
        public string Hashtag { get; set; }

        /// <summary>
        /// From this person
        /// </summary>
        public string PersonFrom { get; set; }

        /// <summary>
        /// To this person
        /// </summary>
        public string PersonTo { get; set; }

        /// <summary>
        /// Person mentioned in tweet
        /// </summary>
        public string PersonReference { get; set; }

        /// <summary>
        /// Tweets with an attitude (Positive, Negative, or Question)
        /// </summary>
        public Attitude Attitude { get; set; }

        /// <summary>
        /// Tweets that contain links
        /// </summary>
        public bool WithLinks { get; set; }

        /// <summary>
        /// Tweets that have been retweeted
        /// </summary>
        public bool WithRetweets { get; set; }

        #region ITwitterEntity Members

        //public T Create<T>(XElement twitterResponse)
        //{
        //    XNamespace atom = "http://www.w3.org/2005/Atom";
        //    XNamespace twitter = "http://api.twitter.com/";
        //    XNamespace openSearch = "http://a9.com/-/spec/opensearch/1.1/";

        //    return new Search
        //    {
        //        Type = Type,
        //        GeoCode = GeoCode,
        //        Page = Page,
        //        PageSize = PageSize,
        //        Query = Query,
        //        ShowUser = ShowUser,
        //        SinceID = SinceID,
        //        SearchLanguage = SearchLanguage,
        //        Locale = Locale,
        //        MaxID = MaxID,
        //        Since = Since,
        //        Until = Until,
        //        ResultType = ResultType,
        //        WordPhrase = WordPhrase,
        //        WordAnd = WordAnd,
        //        WordOr = WordOr,
        //        WordNot = WordNot,
        //        Hashtag = Hashtag,
        //        PersonFrom = PersonFrom,
        //        PersonTo = PersonTo,
        //        PersonReference = PersonReference,
        //        Attitude = Attitude,
        //        WithLinks = WithLinks,
        //        WithRetweets = WithRetweets,
        //        ID = twitterResponse.Element(atom + "id").Value,
        //        Title = twitterResponse.Element(atom + "title").Value,
        //        TwitterWarning =
        //            twitterResponse.Element(twitter + "warning") == null ?
        //            string.Empty :
        //            twitterResponse.Element(twitter + "warning").Value,
        //        Updated = DateTime.Parse(twitterResponse.Element(atom + "updated").Value, CultureInfo.InvariantCulture),
        //        ItemsPerPage =
        //            twitterResponse.Element(openSearch + "itemsPerPage") == null ?
        //            -1 :
        //            int.Parse(twitterResponse.Element(openSearch + "itemsPerPage").Value),
        //        Language =
        //            twitterResponse.Element(openSearch + "language") == null ?
        //            string.Empty :
        //            twitterResponse.Element(openSearch + "language").Value,
        //        Alternate =
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "alternate").Count() == 0 ?
        //            string.Empty :
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "alternate")
        //                .First()
        //                .Attribute("href").Value,
        //        Self =
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "self").Count() == 0 ?
        //            string.Empty :
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "self")
        //                .First()
        //                .Attribute("href").Value,
        //        Search =
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "search").Count() == 0 ?
        //            string.Empty :
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "search")
        //                .First()
        //                .Attribute("href").Value,
        //        Refresh =
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "refresh").Count() == 0 ?
        //            string.Empty :
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "refresh")
        //                .First()
        //                .Attribute("href").Value,
        //        Next =
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "next").Count() == 0 ?
        //            string.Empty :
        //            twitterResponse.Elements(atom + "link")
        //                .Where(elem => elem.Attribute("rel").Value == "next")
        //                .First()
        //                .Attribute("href").Value,
        //        Entries =
        //            (from node in twitterResponse.Nodes()
        //             let atomEntry = node as XElement
        //             where atomEntry != null && atomEntry.Name == atom + "entry"
        //             let author = atomEntry.Element(atom + "author")
        //             select new AtomEntry
        //             {
        //                 ID = atomEntry.Element(atom + "id").Value,
        //                 Published = DateTime.Parse(atomEntry.Element(atom + "published").Value, CultureInfo.InvariantCulture),
        //                 Title = atomEntry.Element(atom + "title").Value,
        //                 Content = atomEntry.Element(atom + "content").Value,
        //                 Updated = DateTime.Parse(atomEntry.Element(atom + "updated").Value, CultureInfo.InvariantCulture),
        //                 Source = atomEntry.Element(twitter + "source").Value,
        //                 Language = atomEntry.Element(twitter + "lang").Value,
        //                 Alternate = atomEntry.Elements(atom + "link")
        //                             .Where(elem => elem.Attribute("rel").Value == "alternate")
        //                             .First()
        //                             .Attribute("href").Value,
        //                 Image = atomEntry.Elements(atom + "link")
        //                             .Where(elem => elem.Attribute("rel").Value == "image")
        //                             .First()
        //                             .Attribute("href").Value,
        //                 Author = new AtomAuthor
        //                 {
        //                     Name = author.Element(atom + "name").Value,
        //                     URI = author.Element(atom + "uri").Value
        //                 },
        //                 Location =
        //                    atomEntry.Element(twitter + "geo") == null ?
        //                    string.Empty :
        //                    atomEntry.Element(twitter + "geo").Value,
        //                 ResultType =
        //                    atomEntry.Element(twitter + "metadata") == null ||
        //                    atomEntry.Element(twitter + "metadata")
        //                        .Element(twitter + "result_type") == null ?
        //                    string.Empty :
        //                    atomEntry
        //                        .Element(twitter + "metadata")
        //                            .Element(twitter + "result_type").Value
        //             }).ToList()
        //    };
        //}

        #endregion
    }
}
