using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
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
        /// replies to your tweets
        /// 
        /// Available Options:
        /// 
        ///     - Since, get tweets replies since this date
        ///     - SinceID, get tweets replies since this ID
        ///     - Page, which page to return
        /// </summary>
        Replies
    }
}
