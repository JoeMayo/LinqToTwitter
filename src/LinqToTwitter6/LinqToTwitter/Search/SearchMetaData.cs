using System.Xml.Serialization;
using System.Text.Json;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class SearchMetaData
    {
        public SearchMetaData() { }
        public SearchMetaData(JsonElement metaData)
        {
            CompletedIn = metaData.GetDecimal("completed_in");
            NextResults = metaData.GetString("next_results");
            Query = metaData.GetString("query");
            RefreshUrl = metaData.GetString("refresh_url");
            Count = metaData.GetInt("count");
            MaxID = (metaData.GetString("max_id_str") ?? string.Empty).GetULong();
            SinceID = (metaData.GetString("since_id_str") ?? string.Empty).GetULong();
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
        public string? NextResults { get; set; }

        /// <summary>
        /// Original Query
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Query string to refresh this search
        /// </summary>
        public string? RefreshUrl { get; set; }

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
