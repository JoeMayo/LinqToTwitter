using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter favorites info
    /// </summary>
    public class Favorites : Status
    {
        /// <summary>
        /// type of favorites to query
        /// </summary>
        [XmlElement(ElementName="FavoritesType")]
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
