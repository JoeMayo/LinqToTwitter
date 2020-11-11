using LinqToTwitter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Mute
    {
        /// <summary>
        /// Type of mute query to perform.
        /// </summary>
        public MuteType Type { get; set; }

        /// <summary>
        /// Allows you to page through query results
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Contains Next and Previous cursors
        /// </summary>
        /// <remarks>
        /// This is read-only and returned with the response
        /// from Twitter. You use it by setting Cursor on the
        /// next request to indicate that you want to move to
        /// either the next or previous page.
        /// </remarks>
        [XmlIgnore]
        public Cursors? CursorMovement { get; internal set; }

        /// <summary>
        /// Set to true for Twitter to return entity metadata with users last tweet.
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Set to true to remove tweet from user entities.
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// List of ids that are muted, populated by IDs query
        /// </summary>
        public List<ulong>? IDList { get; set; }

        /// <summary>
        /// List of User that are muted, populated by List query
        /// </summary>
        public List<User>? Users { get; set; }
    }
}
