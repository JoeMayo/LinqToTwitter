using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter evaluation of tweet quality
    /// </summary>
    public enum FilterLevel
    {
        /// <summary>
        /// No value
        /// </summary>
        None,

        /// <summary>
        /// Low value
        /// </summary>
        Low,

        /// <summary>
        /// Medium quality
        /// </summary>
        Medium,

        /// <summary>
        /// High quality
        /// </summary>
        High
    }
}
