using System;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class SearchMetaData
    {
        public SearchMetaData() { }
        public SearchMetaData(JsonData metaData)
        {
            CompletedIn = metaData.GetValue<decimal>("completed_in");
            MaxID = metaData.GetValue<string>("max_id_str").GetULong(0ul);
            NextResults = metaData.GetValue<string>("next_results");
            Query = metaData.GetValue<string>("query");
            RefreshUrl = metaData.GetValue<string>("refresh_url");
            Count = metaData.GetValue<int>("count");
            SinceID = metaData.GetValue<string>("since_id_str").GetULong(0ul);
        }

        /// <summary>
        /// Processing time for search
        /// </summary>
        public decimal CompletedIn { get; set; }

        /// <summary>
        /// Max ID returned in search
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Query string to get next page
        /// </summary>
        public string NextResults { get; set; }

        /// <summary>
        /// Original Query
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Query string to refresh this search
        /// </summary>
        public string RefreshUrl { get; set; }

        /// <summary>
        /// Number of results per page to return on next query
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Don't return tweets older than this ID
        /// </summary>
        public ulong SinceID { get; set; }
    }
}
