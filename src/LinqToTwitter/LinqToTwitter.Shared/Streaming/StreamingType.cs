using System;

namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// All tweets
        /// </summary>
        Firehose,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample,

        /// <summary>
        /// Activity for multiple users
        /// </summary>
        [Obsolete("Twitter is deprecating this stream on Jun 19, 2018. See the Account Activity API for more info: https://developer.twitter.com/en/docs/accounts-and-users/subscribe-account-activity/overview")]
        Site,

        /// <summary>
        /// A single user's activity
        /// </summary>
        [Obsolete("Twitter is deprecating this stream on Jun 19, 2018. See the Account Activity API for more info: https://developer.twitter.com/en/docs/accounts-and-users/subscribe-account-activity/overview")]
        User
    }
}
