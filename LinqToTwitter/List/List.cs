using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Information for a single Twitter List
    /// </summary>
    [Serializable]
    public class List
    {
        /// <summary>
        /// Type of List query to perform
        /// </summary>
        public ListType Type { get; set; }


        /// <summary>
        /// Helps page results
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// User ID (also used as ListID or Slug in List query)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// List ID or Slug
        /// </summary>
        public string ListID { get; set; }

        /// <summary>
        /// Max ID to retrieve for statuses
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Number of statuses per page
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Page number for statuses
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string ScreenName { get; set; }

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
        /// Catchword for list
        /// </summary>
        public string Slug { get; set; }

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
    }
}
