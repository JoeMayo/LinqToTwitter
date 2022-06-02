namespace LinqToTwitter
{
    public enum TweetType
    {
        /// <summary>
        /// Get tweets from a list
        /// </summary>
        List,

        /// <summary>
        /// Lookup one or more tweets
        /// </summary>
        Lookup,

        /// <summary>
        /// Get the mentions timeline
        /// </summary>
        MentionsTimeline,

        /// <summary>
        /// Most recent tweets and retweets of authenticated user and user follows
        /// </summary>
        ReverseChronologicalTimeline,

        /// <summary>
        /// Get the tweets timeline
        /// </summary>
        TweetsTimeline,

        /// <summary>
        /// Tweets that people shared in a space
        /// </summary>
        SpaceTweets
    }
}
