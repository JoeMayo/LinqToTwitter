using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class SearchMetaData
    {
        public SearchMetaData() { }
        public SearchMetaData(JsonElement metaData)
        {
            CompletedIn = metaData.GetProperty("completed_in").GetDecimal();
            MaxID = metaData.GetProperty("max_id_str").GetUInt64();
            NextResults = metaData.GetProperty("next_results").GetString();
            Query = metaData.GetProperty("query").GetString();
            RefreshUrl = metaData.GetProperty("refresh_url").GetString();
            Count = metaData.GetProperty("count").GetInt32();
            SinceID = metaData.GetProperty("since_id_str").GetUInt64();
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
