using System.Xml.Serialization;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter favorites info
    /// </summary>
    public class Favorites : Status
    {
        public Favorites() { }

        public Favorites(JsonData favJson) : base(favJson) { }

        [XmlIgnore]
        FavoritesType type;

        /// <summary>
        /// type of favorites to query
        /// </summary>
        [XmlIgnore]
        public new FavoritesType Type
        {
            get { return type; }
            set { type = value; }
        }

        [XmlAttribute(AttributeName = "Type")]
        FavoritesType TypeXml
        {
            get { return type; }
            set { type = value; }
        }

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
