namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="TwitterUser"/>
    /// </summary>
    public class UserField
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = 
            "created_at,description,entities,id,location," +
            "name,pinned_tweet_id,profile_image_url,protected," +
            "public_metrics,url,username,verified,withheld";

        /// <summary>
        /// created_at
        /// </summary>
        public const string CreatedAt = "created_at";

        /// <summary>
        /// description
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// entities
        /// </summary>
        public const string Entities = "entities";

        /// <summary>
        /// id
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// location
        /// </summary>
        public const string Location = "location";

        /// <summary>
        /// name
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// pinned_tweet_id
        /// </summary>
        public const string PinnedTweetID = "pinned_tweet_id";

        /// <summary>
        /// profile_image_url
        /// </summary>
        public const string ProfileImageUrl = "profile_image_url";

        /// <summary>
        /// protected
        /// </summary>
        public const string Protected = "protected";

        /// <summary>
        /// public_metrics
        /// </summary>
        public const string PublicMetrics = "public_metrics";

        /// <summary>
        /// url
        /// </summary>
        public const string Url = "url";

        /// <summary>
        /// username
        /// </summary>
        public const string UserName = "username";

        /// <summary>
        /// verified
        /// </summary>
        public const string Verified = "verified";

        /// <summary>
        /// withheld
        /// </summary>
        public const string Withheld = "withheld";
    }
}
