using System.Xml.Serialization;
namespace LinqToTwitter
{
    /// <summary>
    /// Current totals
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Totals
    {
        /// <summary>
        /// Total Updates
        /// </summary>
        public int Updates { get; set; }

        /// <summary>
        /// Total Friends
        /// </summary>
        public int Friends { get; set; }

        /// <summary>
        /// Total Favorites
        /// </summary>
        public int Favorites { get; set; }

        /// <summary>
        /// Total Followers
        /// </summary>
        public int Followers { get; set; }
    }
}
