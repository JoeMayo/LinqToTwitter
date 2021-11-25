using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter.Common.FieldEnums
{
    public class ListFields
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = "created_at,description,follower_count,member_count,owner_id,private";

        /// <summary>
        /// created_at
        /// </summary>
        public const string CreatedAt = "created_at";

        /// <summary>
        /// description
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// follower_count
        /// </summary>
        public const string FollowerCount = "follower_count";

        /// <summary>
        /// member_count
        /// </summary>
        public const string MemberCount = "member_count";

        /// <summary>
        /// owner_id
        /// </summary>
        public const string OwnerID = "owner_id";

        /// <summary>
        /// private
        /// </summary>
        public const string Private = "private";
    }
}
