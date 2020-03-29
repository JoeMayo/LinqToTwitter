using LitJson;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    /// <summary>
    /// User returned by Control Stream query
    /// </summary>
    public class ControlStreamUser
    {
        public ControlStreamUser(JsonData userJson)
        {
            UserID = userJson.GetValue<ulong>("id");
            Name = userJson.GetValue<string>("name");
            DM = userJson.GetValue<bool>("dm");
        }

        /// <summary>
        /// User's unique Twitter ID
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// User's screen name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Does the authenticated user have RW+DM access to user
        /// </summary>
        public bool DM { get; set; }
    }
}
