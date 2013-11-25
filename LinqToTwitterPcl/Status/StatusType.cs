using System;

namespace LinqToTwitter
{
    /// <summary>
    /// type of status request
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// tweets from a specific user
        /// 
        /// Available Options:
        /// 
        ///     - ID, user ID to retrieve tweets for
        ///     - Since, get tweets since this date
        ///     - SinceID, get tweets since this ID
        ///     - Count, number of tweets to retrieve
        ///     - Page, which page to return
        ///     - IncludeRetweets, return retweets too
        /// </summary>
        User,

        /// <summary>
        /// a specific tweet
        /// 
        /// Available Options:
        /// 
        ///     - ID, tweet to retrieve
        /// </summary>
        Show,

        /// <summary>
        /// lists the 20 most recent tweets about the logged-in user
        /// 
        /// Available Options:
        /// 
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        Mentions,

        /// <summary>
        /// Same as Friend, but includes retweets too
        /// 
        /// Available Options:
        /// 
        ///     - Since, get tweets since this date
        ///     - SinceID, get tweets since this ID
        ///     - Count, number of tweets to retrieve
        ///     - Page, which page to return
        /// </summary>
        Home,

        /// <summary>
        /// gets retweets of specified tweet
        /// 
        /// Available Options:
        /// 
        ///     - ID, tweet to get retweets for
        ///     - Count, number of tweets to retrieve
        /// </summary>
        Retweets,

        /// <summary>
        /// lists the 20 most recent re-tweets about the logged-in user
        /// 
        /// Available Options:
        /// 
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        RetweetsOfMe,

        /// <summary>
        /// lists up to 100 ids of users who retweeted a status
        /// 
        /// Available Options:
        /// 
        ///     - ID, retweeted tweet ID
        ///     - Cursor, page to return
        /// </summary>
        Retweeters,

        /// <summary>
        /// provides information, such as HTML, to embed a tweet in a Web page
        /// 
        /// Available Options:
        /// 
        ///     - ID, tweet ID
        ///     - OEmbedUrl, Url of tweet to embed
        ///     - OEmbedMaxWidth, Max number of pixels for width
        ///     - OEmbedHideMedia, Don't initially expand image
        ///     - OEmbedHideThread, Show original message for replies
        ///     - OEmbedOmitScript, Don't include widgets.js script
        ///     - OEmbedAlign, Image alignment: Left, Right, Center, or None
        ///     - OEmbedRelated, Suggested accounts for the viewer to follow
        ///     - OEmbedLanguage, Language code for rendered tweet
        /// </summary>
        Oembed,
    }
}
