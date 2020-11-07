using System.Text.Json;
using System.Xml.Serialization;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter favorites info
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Favorites : Status
    {
        public Favorites() { }

        public Favorites(JsonElement favJson) : base(favJson) { }

        /// <summary>
        /// type of favorites to query
        /// </summary>
        [XmlIgnore]
        public new FavoritesType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// User identity to search (optional)
        /// </summary>
        [XmlIgnore]
        public new ulong UserID { get; set; }

        /// <summary>
        /// Screen name of user to search (optional)
        /// </summary>
        [XmlIgnore]
        public new string ScreenName { get; set; }

        /// <summary>
        /// Number of items to return in a single request (optional)
        /// </summary>
        [XmlIgnore]
        public new int Count { get; set; }

        /// <summary>
        /// Start search at this ID (optional)
        /// </summary>
        [XmlIgnore]
        public new ulong SinceID { get; set; }

        /// <summary>
        /// Don't return results past this ID (optional)
        /// </summary>
        [XmlIgnore]
        public new ulong MaxID { get; set; }

        /// <summary>
        /// Add entities to results (optional)
        /// </summary>
        [XmlIgnore]
        public new bool IncludeEntities { get; set; }

        /// <summary>
        /// Tweets can be compatibility or extended mode. Extended is the 
        /// new mode that allows you to put more characters in a tweet.
        /// </summary>
        [XmlIgnore]
        public new TweetMode TweetMode { get; set; }

        //
        // The following types support XML serialization
        //

        [XmlIgnore]
        FavoritesType type;
        [XmlAttribute(AttributeName = "Type")]
        FavoritesType TypeXml
        {
            get { return type; }
            set { type = value; }
        }

        [XmlIgnore]
        ulong userID;
        [XmlAttribute(AttributeName="UserID")]
        public ulong UserIDXml
        {
            get { return userID; }
            set { userID = value; }
        }

        [XmlIgnore]
        string screenName;
        [XmlAttribute(AttributeName="ScreenName")]
        public string ScreenNameXml
        {
            get { return screenName; }
            set { screenName = value; }
        }

        [XmlIgnore]
        int count;
        [XmlAttribute(AttributeName="Count")]
        public int CountXml
        {
            get { return count; }
            set { count = value; }
        }

        [XmlIgnore]
        ulong sinceID;
        [XmlAttribute(AttributeName="SinceID")]
        public ulong SinceIDXml
        {
            get { return sinceID; }
            set { sinceID = value; }
        }

        [XmlIgnore]
        ulong maxID;
        [XmlAttribute(AttributeName="MaxID")]
        public ulong MaxIDXml
        {
            get { return maxID; }
            set { maxID = value; }
        }
        
        [XmlIgnore]
        bool includeEntities;
        [XmlAttribute(AttributeName="IncludeEntities")]
        public bool IncludeEntitiesXml
        {
            get { return includeEntities; }
            set { includeEntities = value; }
        }
    }
}
