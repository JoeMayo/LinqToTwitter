using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter favorites info
    /// </summary>
    [Serializable]
    public class Favorites : Status
    {
        /// <summary>
        /// type of favorites to query
        /// </summary>
        public new FavoritesType Type { get; set; }

        /// <summary>
        /// User identity to search (optional)
        /// </summary>
        public new string ID { get; set; }

        /// <summary>
        /// Page to retrieve (optional)
        /// </summary>
        public new int Page { get; set; }
    }
}
