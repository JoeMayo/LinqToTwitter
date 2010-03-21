using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// info for query and retrieval of saved searches
    /// </summary>
    [Serializable]
    public class SavedSearch
    {
        /// <summary>
        /// type of search to perform (Searches or Show)
        /// </summary>
        public SavedSearchType Type { get; set; }

        /// <summary>
        /// search item ID
        /// </summary>
        public string ID { get; set; }

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
