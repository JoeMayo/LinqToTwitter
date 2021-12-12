using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Information for a single Twitter List
    /// </summary>
    public class List
    {
        /// <summary>
        /// When created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// What this list is about
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Number of users following this list
        /// </summary>
        [JsonPropertyName("follower_count")]
        public int FollowerCount { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Number of users are members in this list
        /// </summary>
        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        /// <summary>
        /// List Name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// ID of user who is the list owner
        /// </summary>
        [JsonPropertyName("owner_id")]
        public string? OwnerID { get; set; }

        /// <summary>
        /// Can only the owner see this list?
        /// </summary>
        [JsonPropertyName("private")]
        public bool Private { get; set; }
    }
}
