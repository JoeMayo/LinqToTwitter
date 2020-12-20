namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="Tweet"/>
    /// </summary>
    public class TweetField
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = 
            "attachments,author_id,context_annotations,conversation_id," +
            "created_at,entities,geo,id,in_reply_to_user_id,lang,non_public_metrics," +
            "public_metrics,organic_metrics,promoted_metrics,possibly_sensitive," +
            "referenced_tweets,source,text,withheld,reply_settings";

        /// <summary>
        /// All expandable fields, except those requiring permissions
        /// </summary>
        public const string AllFieldsExceptPermissioned =
            "attachments,author_id,context_annotations,conversation_id," +
            "created_at,entities,geo,id,in_reply_to_user_id,lang,public_metrics," +
            "possibly_sensitive,referenced_tweets,source,text,withheld,reply_settings";

        /// <summary>
        /// attachments
        /// </summary>
        public const string Attachments = "attachments";

        /// <summary>
        /// author_id
        /// </summary>
        public const string AuthorID = "author_id";

        /// <summary>
        /// context_annotations
        /// </summary>
        public const string ContextAnnotations = "context_annotations";

        /// <summary>
        /// conversation_id
        /// </summary>
        public const string ConversationID = "conversation_id";

        /// <summary>
        /// created_at
        /// </summary>
        public const string CreatedAt = "created_at";

        /// <summary>
        /// entities
        /// </summary>
        public const string Entities = "entities";

        /// <summary>
        /// geo
        /// </summary>
        public const string Geo = "geo";

        /// <summary>
        /// id
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// in_reply_to_user_id
        /// </summary>
        public const string InReplyToUserID = "in_reply_to_user_id";

        /// <summary>
        /// lang
        /// </summary>
        public const string Language = "lang";

        /// <summary>
        /// non_public_metrics
        /// </summary>
        public const string NonPublicMetrics = "non_public_metrics";

        /// <summary>
        /// public_metrics
        /// </summary>
        public const string PublicMetrics = "public_metrics";

        /// <summary>
        /// organic_metrics
        /// </summary>
        public const string OrganicMetrics = "organic_metrics";

        /// <summary>
        /// promoted_metrics
        /// </summary>
        public const string PromotedMetrics = "promoted_metrics";

        /// <summary>
        /// possibly_sensitive
        /// </summary>
        public const string PossiblySensitive = "possibly_sensitive";

        /// <summary>
        /// referenced_tweets
        /// </summary>
        public const string ReferencedTweets = "referenced_tweets";

        /// <summary>
        /// who can reply to a tweet
        /// </summary>
        public const string ReplySettings = "reply_settings";

        /// <summary>
        /// source
        /// </summary>
        public const string Source = "source";

        /// <summary>
        /// text
        /// </summary>
        public const string Text = "text";

        /// <summary>
        /// withheld
        /// </summary>
        public const string Withheld = "withheld";
    }
}
