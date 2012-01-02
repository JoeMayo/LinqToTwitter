using System.Collections.Generic;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Information for a single Twitter List
    /// </summary>
    public class List
    {
        public static List CreateList(XElement list, XElement cursorNode)
        {
            return new List
            {
                ListIDResult = list.Element("id").Value,
                Name = list.Element("name").Value,
                FullName = list.Element("full_name").Value,
                SlugResult = list.Element("slug").Value,
                Description = list.Element("description").Value,
                SubscriberCount = int.Parse(list.Element("subscriber_count").Value),
                MemberCount = int.Parse(list.Element("member_count").Value),
                Uri = list.Element("uri").Value,
                Mode = list.Element("mode").Value,
                Users = new List<User>
                {
                    User.CreateUser(list.Element("user"))
                },
                CursorMovement = Cursors.CreateCursors(cursorNode)
            };
        }

        /// <summary>
        /// Type of List query to perform (Query Filter)
        /// </summary>
        public ListType Type { get; set; }

        /// <summary>
        /// Helps page results (Query Filter)
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// User ID (Query Filter)
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// List ID (Query Filter)
        /// </summary>
        public string ListID { get; set; }

        /// <summary>
        /// List ID (Returned from Twitter)
        /// </summary>
        public string ListIDResult { get; set; }

        /// <summary>
        /// Catchword for list (Query Filter)
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Catchword for list (Returned from Twitter)
        /// </summary>
        public string SlugResult { get; set; }

        /// <summary>
        /// ID of List Owner (Query Filter)
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// ScreenName of List Owner (Query Filter)
        /// </summary>
        public string OwnerScreenName { get; set; }

        /// <summary>
        /// Max ID to retrieve for statuses (Query Filter)
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Number of statuses per page (Query Filter)
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Page number for statuses (Query Filter)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Truncate all user info, except for ID (Query Filter)
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Add entities to tweets (Query Filter)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Add retweets, in addition to normal tweets (Query Filter)
        /// </summary>
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Statuses since status ID (Query Filter)
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// Short name of List (Returned from Twitter)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fully qualified name of list (Returned from Twitter)
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Description of List's purpose (Returned from Twitter)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of subscribers (Returned from Twitter)
        /// </summary>
        public int SubscriberCount { get; set; }

        /// <summary>
        /// Number of members (Returned from Twitter)
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Uri of List (Returned from Twitter)
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// List mode (Returned from Twitter)
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Users associated with List (Returned from Twitter)
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Statuses for list (Returned from Twitter)
        /// </summary>
        public List<Status> Statuses { get; set; }

        /// <summary>
        /// Cursors for current request (Returned from Twitter)
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// Only returns lists that belong to authenticated 
        /// user or user identified by ID or ScreenName (Query Filter)
        /// </summary>
        public bool FilterToOwnedLists { get; set; }
    }
}
