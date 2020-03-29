using System;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// info for query and retrieval of saved searches
    /// </summary>
    public class SavedSearch
    {
        public SavedSearch() { }
        public SavedSearch(JsonData searchJson)
        {
            Query = searchJson.GetValue<string>("query");
            Name = searchJson.GetValue<string>("name");
            Postition = searchJson.GetValue<int>("position");
            IDString = searchJson.GetValue<string>("id_str");
            CreatedAt = searchJson.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
        }

        /// <summary>
        /// type of search to perform (Searches or Show)
        /// </summary>
        public SavedSearchType Type { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        public string IDString { get; set; }

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
        public int Postition { get; set; }

        /// <summary>
        /// when search was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
