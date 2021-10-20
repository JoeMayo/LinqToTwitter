namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="Tweet"/> and <see cref="TwitterUser"/> objects
    /// </summary>
    public class ExpansionField
    {
        /// <summary>
        /// All expandable fields on <see cref="Tweet"/>
        /// </summary>
        public const string AllTweetFields = 
            "attachments.poll_ids,attachments.media_keys,author_id," +
            "entities.mentions.username,geo.place_id,in_reply_to_user_id," +
            "referenced_tweets.id,referenced_tweets.id.author_id";

        /// <summary>
        /// All expandable fields on <see cref="TwitterUser"/>
        /// </summary>
        public const string AllUserFields = "pinned_tweet_id";

        /// <summary>
        /// All expandable fields on <see cref="Space"/>
        /// </summary>
        public const string AllSpaceFields = "creator_id,host_ids,invited_user_ids,speaker_ids";

        /// <summary>
        /// <see cref="Tweet"/> - author_id
        /// </summary>
        public const string AuthorID = "author_id";

        /// <summary>
        /// ID of person who created a <see cref="Space"/>
        /// </summary>
        public const string CreatorID = "creator_id";

        /// <summary>
        /// ID of people hosting a <see cref="Space"/>
        /// </summary>
        public const string HostIds = "host_ids";

        /// <summary>
        /// <see cref="Tweet"/> - in_reply_to_user_id
        /// </summary>
        public const string InReplyToUserID = "in_reply_to_user_id";

        /// <summary>
        /// IDs of users who were invited to a <see cref="Space"/>
        /// </summary>
        public const string InvitedUserIds = "invited_user_ids";

        /// <summary>
        /// <see cref="Tweet"/> - attachments.media_keys
        /// </summary>
        public const string MediaKeys = "attachments.media_keys";

        /// <summary>
        /// <see cref="Tweet"/> - entities.mentions.username
        /// </summary>
        public const string MentionsUsername = "entities.mentions.username";

        /// <summary>
        /// <see cref="TwitterUser"/> - pinned_tweet_id
        /// </summary>
        public const string PinnedTweetID = "pinned_tweet_id";

        /// <summary>
        /// <see cref="Tweet"/> - geo.place_id
        /// </summary>
        public const string PlaceID = "geo.place_id";

        /// <summary>
        /// <see cref="Tweet"/> - attachments.poll_ids
        /// </summary>
        public const string PollIds = "attachments.poll_ids";

        /// <summary>
        /// <see cref="Tweet"/> - referenced_tweets.id
        /// </summary>
        public const string ReferencedTweetID = "referenced_tweets.id";

        /// <summary>
        /// <see cref="Tweet"/> - referenced_tweets.id.author_id
        /// </summary>
        public const string ReferencedTweetAuthorID = "referenced_tweets.id.author_id";

        /// <summary>
        /// IDs of people who are speakers at a <see cref="Space"/>
        /// </summary>
        public const string SpeakerIds = "speaker_ids";
    }
}
