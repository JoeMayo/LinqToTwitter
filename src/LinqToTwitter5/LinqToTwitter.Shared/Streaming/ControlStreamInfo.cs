using LinqToTwitter.Common;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from ControlStreams Info query
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class ControlStreamInfo
    {
        public ControlStreamInfo(JsonData infoJson)
        {
            var info = infoJson.GetValue<JsonData>("info");
            Users =
                (from JsonData user in info.GetValue<JsonData>("users")
                 select new ControlStreamUser(user))
                .ToList();
            Delimited = info.GetValue<string>("delimited");
            IncludeFollowingsActivity = info.GetValue<bool>("include_followings_activity");
            IncludeUserChanges = info.GetValue<bool>("include_user_changes");
            Replies = info.GetValue<string>("replies");
            With = info.GetValue<string>("with");
        }

        /// <summary>
        /// List of users on stream
        /// </summary>
        public List<ControlStreamUser> Users { get; set; }

        /// <summary>
        /// Whether stream is using delimeters
        /// </summary>
        public string Delimited { get; set; }

        /// <summary>
        /// If stream returns followers activity
        /// </summary>
        public bool IncludeFollowingsActivity { get; set; }

        /// <summary>
        /// If stream includes user changes
        /// </summary>
        public bool IncludeUserChanges { get; set; }

        /// <summary>
        /// Set to "all" to include replies
        /// </summary>
        public string Replies { get; set; }

        /// <summary>
        /// Type of messages: "followings" include followers and "user" is onlly user
        /// </summary>
        public string With { get; set; }
    }
}
