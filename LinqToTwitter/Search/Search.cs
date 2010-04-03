using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// for performing Twitter searches
    /// </summary>
    [Serializable]
    public class Search : AtomFeed
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
    }
}
