using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// All public tweets
        /// </summary>
        Firehose,

        /// <summary>
        /// Tweets containing http or https
        /// </summary>
        Links,

        /// <summary>
        /// Retweets...
        /// </summary>
        Retweet,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample
    }
}
