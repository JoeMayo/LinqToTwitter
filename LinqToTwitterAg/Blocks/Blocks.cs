using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// helps retrieve information about blocks
    /// </summary>
    public class Blocks
    {
        //
        // Input parameters
        //

        /// <summary>
        /// type of blocks request to perform (input only)
        /// </summary>
        public BlockingType Type { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name (input only)
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id (input only)
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// page to retrieve (input only)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page to return (input only)
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Don't include statuses in response (input only)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Identifier for previous or next page to query (input only)
        /// </summary>
        public string Cursor { get; set; }

        //
        // Output parameters
        //

        /// <summary>
        /// Prev/Next cursor to move through ID and User lists.
        /// </summary>
        public Cursors Cursors { get; set; }

        /// <summary>
        /// Listed Count
        /// </summary>
        public int ListedCount { get; set; }

        /// <summary>
        /// List of blocked IDs
        /// </summary>
        public List<string> IDs { get; set; }

        /// <summary>
        /// user being blocked
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Populated for blocking queries, showing all blocked users
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Twitter response for no block on specified user
        /// </summary>
        public TwitterHashResponse NoBlock { get; set; }
    }
}
