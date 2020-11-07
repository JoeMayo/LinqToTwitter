using System;
using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// info for query and retrieval of saved searches
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class SavedSearch
    {
        public SavedSearch() { }
        public SavedSearch(JsonElement searchJson)
        {
            Query = searchJson.GetString("query");
            Name = searchJson.GetString("name");
            Position = searchJson.GetInt("position");
            IDResponse = searchJson.GetUlong("id");
            CreatedAt = searchJson.GetString("created_at").GetDate(DateTime.MaxValue);
        }

        /// <summary>
        /// type of search to perform (Searches or Show)
        /// </summary>
        public SavedSearchType Type { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// Search item ID.
        /// </summary>
        public ulong IDResponse { get; set; }

        /// <summary>
        /// name of search
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// search query contents
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// position in search list
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// when search was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
