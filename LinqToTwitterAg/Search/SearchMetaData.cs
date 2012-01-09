using System;

namespace LinqToTwitter
{
    public class SearchMetaData
    {
        /// <summary>
        /// Number of recent retweets
        /// </summary>
        public int RecentRetweets { get; set; }

        /// <summary>
        /// Type of search result (i.e. Mixed, Recent, or Popular)
        /// </summary>
        public ResultType ResultType { get; set; }
    }
}
