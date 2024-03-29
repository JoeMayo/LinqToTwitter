﻿namespace LinqToTwitter
{
    public enum TweetType
    {
        /// <summary>
        /// Get bookmarked tweets
        /// </summary>
        Bookmarks,

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
        /// Get quotes of a specific tweet
        /// </summary>
        QuoteTweets,

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
