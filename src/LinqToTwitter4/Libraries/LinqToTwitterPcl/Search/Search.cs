using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// for performing Twitter searches
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Search
    {
        //
        // Input parameters
        //

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
        public int Count { get; set; }

        /// <summary>
        /// Return tweets before this date
        /// </summary>
        public DateTime Until { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// first status ID
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        public string GeoCode { get; set; }

        /// <summary>
        /// Metadata for type of result requested (mixed, recent, or popular)
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        /// Include entities in results
        /// </summary>
        public bool IncludeEntities { get; set; }

        //
        // Output results
        //

        /// <summary>
        /// Tweet data returned from the search
        /// </summary>
        public List<Status> Statuses { get; set; }

        /// <summary>
        /// Tweet metadata returned from search
        /// </summary>
        public SearchMetaData SearchMetaData { get; set; }
    }
}
