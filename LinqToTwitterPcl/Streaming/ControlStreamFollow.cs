
using LinqToTwitter.Common;
using LitJson;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from ControlStreams Followers query
    /// </summary>
    public class ControlStreamFollow
    {
        public ControlStreamFollow(JsonData csJson)
        {
            var follow = csJson.GetValue<JsonData>("follow");
            User = new ControlStreamUser(follow.GetValue<JsonData>("user"));
            Friends =
                (from JsonData friend in follow.GetValue<JsonData>("friends")
                 select (ulong)friend)
                .ToList();
            Cursors = new Cursors(follow);
        }

        /// <summary>
        /// User to get followers for
        /// </summary>
        public ControlStreamUser User { get; set; }

        /// <summary>
        /// User's friends
        /// </summary>
        public List<ulong> Friends { get; set; }

        /// <summary>
        /// Cursors for paging through friends results
        /// </summary>
        public Cursors Cursors { get; set; }
    }
}
