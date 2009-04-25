using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// for performing Twitter searches
    /// </summary>
    public class TwitterSearch
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
        /// language to search in (ISO 639-1)
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// number of results for each page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// last status ID
        /// </summary>
        public int SinceID { get; set; }

        /// <summary>
        /// location, specified as "latitude,longitude,radius"
        /// </summary>
        public string GeoCode { get; set; }

        /// <summary>
        /// adds user information for each tweet if true (default = false)
        /// </summary>
        public string ShowUser { get; set; }

        /// <summary>
        /// search results
        /// </summary>
        public AtomFeed SearchResults { get; set; }
    }
}
