namespace LinqToTwitter
{
    public class TweetExcludes
    {
        /// <summary>
        /// Exclude replies and retweets
        /// </summary>
        public const string All = "replies,retweets";

        /// <summary>
        /// Exclude Replies
        /// </summary>
        public const string Replies = "replies";

        /// <summary>
        /// Exclude Retweets
        /// </summary>
        public const string Retweets = "retweets";
    }
}
