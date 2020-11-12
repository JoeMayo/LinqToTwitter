using System;
using System.Collections.Generic;
using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// Information for a single Twitter List
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class List
    {
        public List() 
        {
            Statuses = new List<Status>();
        }

        public List(JsonElement element) : this()
        {
            Name = element.GetString("name");
            FullName = element.GetString("full_name");
            MemberCount = element.GetInt("member_count");
            Description = element.GetString("description");
            Mode = element.GetString("mode");
            Uri = element.GetString("uri");
            Users = new List<User> { new User(element.GetProperty("user")) };
            ListIDResponse = element.GetUlong("id");
            SubscriberCount = element.GetInt("subscriber_count");
            CreatedAt = element.GetString("created_at")?.GetDate(DateTime.MaxValue);
            Following = element.GetBool("following");
            SlugResponse = element.GetString("slug");
        }

        /// <summary>
        /// Type of List query to perform (Query Filter)
        /// </summary>
        public ListType? Type { get; set; }

        /// <summary>
        /// Helps page results (Query Filter)
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// User ID (Query Filter)
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// ScreenName of user for query
        /// </summary>
        public string? ScreenName { get; set; }

        /// <summary>
        /// List ID (Query Filter)
        /// </summary>
        public ulong ListID { get; set; }

        /// <summary>
        /// List ID (Returned from Twitter)
        /// </summary>
        public ulong ListIDResponse { get; set; }

        /// <summary>
        /// Catchword for list (Query Filter)
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Catchword for list (Returned from Twitter)
        /// </summary>
        public string? SlugResponse { get; set; }

        /// <summary>
        /// ID of List Owner (Query Filter)
        /// </summary>
        public ulong OwnerID { get; set; }

        /// <summary>
        /// ScreenName of List Owner (Query Filter)
        /// </summary>
        public string? OwnerScreenName { get; set; }

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
        /// Add entities to tweets (Query Filter, default: true)
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
        /// Don't include statuses in response (Query Filter)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Causes Twitter to return the lists owned by the authenticated user first (Query Filter)
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Short name of List (Returned from Twitter)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Fully qualified name of list (Returned from Twitter)
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Description of List's purpose (Returned from Twitter)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Number of subscribers (Returned from Twitter)
        /// </summary>
        public int SubscriberCount { get; set; }

        /// <summary>
        /// When the list was created
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Is authenticated user following list
        /// </summary>
        public bool Following { get; set; }

        /// <summary>
        /// Number of members (Returned from Twitter)
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Uri of List (Returned from Twitter)
        /// </summary>
        public string? Uri { get; set; }

        /// <summary>
        /// List mode (Returned from Twitter)
        /// </summary>
        public string? Mode { get; set; }

        /// <summary>
        /// Users associated with List (Returned from Twitter)
        /// </summary>
        public List<User>? Users { get; set; }

        /// <summary>
        /// Statuses for list (Returned from Twitter)
        /// </summary>
        public List<Status>? Statuses { get; set; }

        /// <summary>
        /// Cursors for current request (Returned from Twitter)
        /// </summary>
        public Cursors? CursorMovement { get; set; }

        /// <summary>
        /// Only returns lists that belong to authenticated 
        /// user or user identified by ID or ScreenName (Query Filter)
        /// </summary>
        public bool FilterToOwnedLists { get; set; }
    }
}
