using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                ListID = list.Element("id").Value,
                Name = list.Element("name").Value,
                FullName = list.Element("full_name").Value,
                Slug = list.Element("slug").Value,
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
        /// Type of List query to perform
        /// </summary>
        public ListType Type { get; set; }

        /// <summary>
        /// Helps page results
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        public string ListID { get; set; }

        /// <summary>
        /// Catchword for list
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// ID of List Owner
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// ScreenName of List Owner
        /// </summary>
        public string OwnerScreenName { get; set; }

        /// <summary>
        /// Max ID to retrieve for statuses
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Number of statuses per page
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Page number for statuses
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Truncate all user info, except for ID
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Add entities to tweets
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Add retweets, in addition to normal tweets
        /// </summary>
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Statuses since status ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// Short name of List
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fully qualified name of list
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Description of List's purpose
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of subscribers
        /// </summary>
        public int SubscriberCount { get; set; }

        /// <summary>
        /// Number of members
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Uri of List
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// List mode
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Users associated with List
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Statuses for list
        /// </summary>
        public List<Status> Statuses { get; set; }

        /// <summary>
        /// Cursors for current request
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// Only returns lists that belong to authenticated 
        /// user or user identified by ID or ScreenName
        /// </summary>
        public bool FilterToOwnedLists { get; set; }
    }
}
