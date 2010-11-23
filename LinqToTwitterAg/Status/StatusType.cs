using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// type of status request
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// tweets of all users on Twitter
        /// 
        /// - default tweet type with no options
        /// </summary>
        Public,

        /// <summary>
        /// tweets from friends
        /// 
        /// Available Options:
        /// 
        ///     - Since, get tweets since this date
        ///     - SinceID, get tweets since this ID
        ///     - Count, number of tweets to retrieve
        ///     - Page, which page to return
        /// </summary>
        Friends,

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
        /// lists the 20 most recent re-tweets by the logged-in user
        /// 
        /// Available Options:
        /// 
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        RetweetedByMe,

        /// <summary>
        /// lists the 20 most recent re-tweets to the logged-in user
        /// 
        /// Available Options:
        /// 
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        RetweetedToMe,


        /// <summary>
        /// lists the 20 most recent re-tweets by the specified user
        /// 
        /// Available Options:
        /// 
        ///     - ID, ID or ScreenName of User to get Retweets for
        ///     - UserID, ID of User to get Retweets for
        ///     - ScreenName, Name of user to get Retweets for
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        RetweetedToUser,

        /// <summary>
        /// lists the 20 most recent re-tweets to the specified user
        /// 
        /// Available Options:
        /// 
        ///     - ID, ID or ScreenName of User to get Retweets for
        ///     - UserID, ID of User to get Retweets for
        ///     - ScreenName, Name of user to get Retweets for
        ///     - SinceID, get tweets since this ID
        ///     - MaxID, gets tweets less than this ID
        ///     - Count, max number of tweets to return
        ///     - Page, which page to return
        /// </summary>
        RetweetedByUser,

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

    }
}
